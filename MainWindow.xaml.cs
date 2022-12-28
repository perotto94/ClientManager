using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GlobalVars globalVars;

        // If gui update allowed
        bool updateAllowed = true;

        // Shift calendar one week
        int shiftCalendar = 0;

        // Shift clients up/down
        int shiftClients = 0;

        // List of currently shown clients
        readonly List<Client> currentClients = new();

        // List of currently shown data events
        readonly List<List<DataEvent>> currentEvents;

        // Selected event
        PlannedEvent selectedEvent = null;

        // Media player
        private MediaPlayer mediaPlayer = new();

        // Assembly resource names
        readonly List<string> resources = new();

        public MainWindow(GlobalVars globalVars)
        {
            InitializeComponent();

            this.globalVars = globalVars;

            currentEvents = new(14);
            for (int i = 0; i < 14; i++)
            {
                currentEvents.Add(new());
            }

            UpdateContent();

            DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += PlayReminders;
            timer.Start();
        }

        /// <summary>
        /// Pley reminder sounds
        /// </summary>
        private void PlayReminders(object sender, EventArgs e)
        {
            if (!updateAllowed) return;

            DateTime now = DateTime.Now;
            TimeSpan diff = TimeSpan.FromMinutes(2);
            DateTime timeMin, timeMax;

            foreach (Database db in globalVars.DB)
            {
                foreach (Client client in db.Clients)
                {
                    foreach (Order order in client.Orders)
                    {
                        if (order is OrderRegular orderRegular)
                        {
                            foreach (Exercise exercise in orderRegular.Exercises)
                            {
                                foreach (Reminder reminder in exercise.Reminders)
                                {
                                    if (!reminder.Happened && exercise.DateRelevant != DateTime.MaxValue)
                                    {
                                        timeMin = exercise.DateStart - reminder.TimeToEvent - diff;
                                        timeMax = exercise.DateStart - reminder.TimeToEvent + diff;

                                        if (now > timeMin && now < timeMax)
                                        {
                                            PlaySound(reminder.SoundFileName);
                                            reminder.Happened = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (PlannedEvent plannedEvent in db.Events)
                {
                    foreach (Reminder reminder in plannedEvent.Reminders)
                    {
                        if (!reminder.Happened && plannedEvent.DateRelevant != DateTime.MaxValue)
                        {
                            timeMin = plannedEvent.DateStart - reminder.TimeToEvent - diff;
                            timeMax = plannedEvent.DateStart - reminder.TimeToEvent + diff;

                            if (now > timeMin && now < timeMax)
                            {
                                PlaySound(reminder.SoundFileName);
                                reminder.Happened = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Play sound by name
        /// </summary>
        private void PlaySound(string name)
        {
            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Load the embedded resource as a stream
            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.wav.{name}.wav");

            // Load the stream into the player
            var player = new SoundPlayer(stream);

            // Play the sound
            player.Play();
        }

        /// <summary>
        /// Open new window to add database
        /// </summary>
        private void buttonAddDB_Click(object sender, RoutedEventArgs e)
        {
            WindowAddDatabase window = new(globalVars);
            window.ShowDialog();

            if (window.DialogResult == false)
            {
                return;
            }
        }

        /// <summary>
        /// Open new window to add client
        /// </summary>
        private void buttonAddClient_Click(object sender, RoutedEventArgs e)
        {
            updateAllowed = false;

            if (globalVars.DB.Count < 1)
            {
                InfoBox infoBox = new("Нет ни одной базы данных. Нельзя добавить клиента");
                infoBox.ShowDialog();

                updateAllowed = true;

                return;
            }

            WindowAddClient window = new(globalVars);
            window.ShowDialog();

            updateAllowed = true;

            UpdateContent();
        }

        /// <summary>
        /// Update content according to databases
        /// </summary>
        private void UpdateContent()
        {
            if (!updateAllowed)
            {
                return;
            }

            updateAllowed = false;

            ClearLBSelection();
            UpdateClients();
            UpdateRTBDaysOfWeek();
            ExportDB();

            updateAllowed = true;
        }

        /// <summary>
        /// Update canvas with db data
        /// </summary>
        private void UpdateClients()
        {
            // Clear current clients
            currentClients.Clear();

            // Get list of clients sorted by DueDate
            List<Client> clients = globalVars.GetClients(true);

            // Number of clients
            int clientsNum = clients.Count;

            // Current datetime
            DateTime now = DateTime.Now;

            // Fill window with client info
            for (int i = 0; i < 6; i++)
            {
                // Client index
                int j = i + shiftClients * 3;

                // Get GUI items
                var rtbClient = (RichTextBox)FindName($"rtbClient_{i}");
                var lbOrders = (ListBox)FindName($"lbOrders_{i}");
                var rtbOrder = (RichTextBox)FindName($"rtbOrder_{i}");
                var lbExercises = (ListBox)FindName($"lbExercises_{i}");
                var rtbExercise = (RichTextBox)FindName($"rtbExercise_{i}");
                var grid = (Grid)FindName($"gridClient_{i}");

                // Clear GUI items
                rtbClient.Document.Blocks.Clear();
                lbOrders.Items.Clear();
                rtbOrder.Document.Blocks.Clear();
                lbExercises.Items.Clear();
                rtbExercise.Document.Blocks.Clear();

                // Check if j-th client exists, otherwise pass the cycle
                if (j > clientsNum - 1)
                {
                    grid.Background = new SolidColorBrush(Colors.LightGray);

                    continue;
                }

                // Current client, add to list
                Client client = clients[j];
                currentClients.Add(client);

                // Update GUI
                rtbClient.Document = client.ToFlowDocument();

                foreach (Order order in client.Orders)
                {
                    lbOrders.Items.Add($"{order.ShortDescription()}");
                }

                if (client.Orders.Count > 0)
                {
                    lbOrders.SelectedIndex = 0;
                }

                grid.Background = new SolidColorBrush(Colors.Azure);

                if (client.DateRelevant != DateTime.MaxValue && client.DateRelevant.Year == now.Year)
                {
                    if (client.DateRelevant.DayOfYear < now.DayOfYear)
                    {
                        grid.Background = new SolidColorBrush(Color.FromRgb((byte)(255 - 20 * i), 0, 0));
                    }
                    else if (client.DateRelevant.DayOfYear == now.DayOfYear)
                    {
                        grid.Background = new SolidColorBrush(Color.FromRgb((byte)(28 + 20 * i), 255, 124));
                    }
                    else if (client.DateRelevant.DayOfYear > now.DayOfYear)
                    {
                        grid.Background = new SolidColorBrush(Color.FromRgb(86, (byte)(124 - 20 * i), 255));
                    }
                }
            }
        }

        /// <summary>
        /// Export databases to txt files
        /// </summary>
        private void ExportDB()
        {
            if (globalVars.DB.Count < 1)
            {
                return;
            }

            bool result = Database.WriteToFile(globalVars.DB, out string message);

            if (result)
            {
                circleSyncStatus.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                circleSyncStatus.Fill = new SolidColorBrush(Colors.Red);
            }

            tbSyncMessage.Text = message;
        }

        /// <summary>
        /// Update days of week
        /// </summary>
        private void UpdateRTBDaysOfWeek()
        {
            DateTime dateTimeNow = DateTime.Now;
            DateTime dateTime = dateTimeNow + new TimeSpan(7 * shiftCalendar, 0, 0, 0);

            int today = (int)dateTime.DayOfWeek - 1;
            if (today == -1) today = 6;

            string[] dayNames = { "ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС" };

            for (int i = 0; i < 14; i++)
            {
                var rtbDay = (RichTextBox)FindName($"rtbDay{i}");
                var lbData = (ListBox)FindName($"lbData{i:D2}");

                Paragraph paragraphDay = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Center };
                paragraphDay.Inlines.Add(new Bold(new Run(dayNames[i % 7])));

                Paragraph paragraphDate = new() { FontSize = 9, LineHeight = 1, TextAlignment = TextAlignment.Center };

                DateTime dateTimeDay;

                if (i < today)
                {
                    dateTimeDay = dateTime - new TimeSpan(today - i, 0, 0, 0);
                }
                else if (i > today)
                {
                    dateTimeDay = dateTime + new TimeSpan(i - today, 0, 0, 0);
                }
                else
                {
                    dateTimeDay = dateTime;
                }

                if (dateTimeDay < dateTimeNow)
                {
                    rtbDay.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (dateTimeDay > dateTimeNow)
                {
                    rtbDay.Background = new SolidColorBrush(Colors.AliceBlue);
                }
                else
                {
                    rtbDay.Background = new SolidColorBrush(Colors.LawnGreen);
                }

                paragraphDate.Inlines.Add(new Run($"{dateTimeDay:dd.MM}"));

                FlowDocument flowDocument = new();
                flowDocument.Blocks.Add(paragraphDay);
                flowDocument.Blocks.Add(paragraphDate);

                rtbDay.Document = flowDocument;

                UpdateLBData(lbData, i, dateTimeDay);
            }
        }

        /// <summary>
        /// Contains order/event data for GUI
        /// </summary>
        class DataEvent
        {
            public PlannedEvent Event = null;

            public DateTime Start { get; set; } = DateTime.MinValue;

            public DateTime End { get; set; } = DateTime.MinValue;

            public int Order { get => Start.Hour * 60 + Start.Minute; }

            public string Description { get; set; } = "";

            public override string ToString()
            {
                if (End == DateTime.MinValue || End.DayOfYear != Start.DayOfYear)
                {
                    return $"{Start:HH:mm}  {Description}";
                }
                else
                {
                    return $"{Start:HH:mm} - {End:HH:mm}  {Description}";
                }
            }
        }

        /// <summary>
        /// Update calendar text boxes
        /// </summary>
        private void UpdateLBData(ListBox lb, int indexDay, DateTime dateTime)
        {
            currentEvents[indexDay].Clear();

            foreach (Database db in globalVars.DB)
            {
                foreach (Client client in db.Clients)
                {
                    foreach (Order order in client.Orders)
                    {
                        if (order is OrderSingle orderSingle)
                        {
                            if (dateTime.DayOfYear == orderSingle.DueDate.DayOfYear)
                            {
                                currentEvents[indexDay].Add(new DataEvent()
                                {
                                    Start = orderSingle.DueDate,
                                    Description = $"{client.Name} / {orderSingle.Subject}"
                                });
                            }
                        }
                        else if (order is OrderRegular orderRegular)
                        {
                            foreach (Exercise exercise in orderRegular.Exercises)
                            {
                                if (dateTime.DayOfYear == exercise.DateStart.DayOfYear)
                                {
                                    currentEvents[indexDay].Add(new DataEvent()
                                    {
                                        Start = exercise.DateStart,
                                        End = exercise.DateEnd,
                                        Description = $"{client.Name} / {orderRegular.Subject}"
                                    });
                                }
                            }
                        }
                    }
                }

                foreach (PlannedEvent plannedEvent in db.Events)
                {
                    if (dateTime.DayOfYear == plannedEvent.DateStart.DayOfYear)
                    {
                        currentEvents[indexDay].Add(new DataEvent()
                        {
                            Event = plannedEvent,
                            Start = plannedEvent.DateStart,
                            End = plannedEvent.DateEnd,
                            Description = plannedEvent.Description
                        });
                    }
                }
            }

            currentEvents[indexDay].Sort((x, y) => x.Order.CompareTo(y.Order));

            FlowDocument flowDocument = new();

            lb.Items.Clear();

            foreach (DataEvent dataEvent in currentEvents[indexDay])
            {
                Paragraph paragraph = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };

                paragraph.Inlines.Add(new Run(dataEvent.ToString()));

                flowDocument.Blocks.Add(paragraph);

                lb.Items.Add(dataEvent.ToString());
            }
        }

        /// <summary>
        /// Add event
        /// </summary>
        private void buttonAddEvent_Click(object sender, RoutedEventArgs e)
        {
            updateAllowed = false;

            if (globalVars.DB.Count < 1)
            {
                InfoBox infoBox = new("Нет ни одной базы данных. Нельзя добавить событие");
                infoBox.ShowDialog();

                updateAllowed = true;

                return;
            }

            WindowAddEvent window = new(globalVars);
            window.ShowDialog();

            updateAllowed = true;

            UpdateContent();
        }

        /// <summary>
        /// Shift calendar to the left
        /// </summary>
        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            shiftCalendar--;

            UpdateContent();
        }

        /// <summary>
        /// Shift calendar to the right
        /// </summary>
        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            shiftCalendar++;

            UpdateContent();
        }

        /// <summary>
        /// Shift clients up
        /// </summary>
        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            if (shiftClients > 0)
            {
                shiftClients--;

                UpdateContent();
            }
        }

        /// <summary>
        /// Shift clients down
        /// </summary>
        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            if (shiftClients * 3 + 6 < globalVars.GetNumberOfClients())
            {
                shiftClients++;

                UpdateContent();
            }
        }

        /// <summary>
        /// Edit client button
        /// </summary>
        private void butClientEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count)
            {
                updateAllowed = true;

                return;
            }

            WindowAddClient window = new(globalVars, true, currentClients[indexClient]);
            window.ShowDialog();

            updateAllowed = true;

            if (window.DialogResult == true)
            {
                UpdateContent();
            }
        }

        /// <summary>
        /// Change selected order listbox
        /// </summary>
        private void lbOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox lb) return;

            updateAllowed = false;

            int indexClient = lb.Name[^1] - '0';

            if (indexClient >= currentClients.Count)
            {
                updateAllowed = true;

                return;
            }

            var rtb = (RichTextBox)FindName($"rtbOrder_{indexClient}");

            var order = currentClients[indexClient][lb.SelectedIndex];

            rtb.Document = order.ToFlowDocument();

            var lbe = (ListBox)FindName($"lbExercises_{indexClient}");

            if (order is OrderRegular orderRegular)
            {
                if (orderRegular.Exercises.Count > 0)
                {
                    foreach (Exercise exercise in orderRegular.Exercises)
                    {
                        lbe.Items.Add(exercise.ShortDescription());
                    }

                    lbe.SelectedIndex = 0;
                }
            }
            else
            {
                lbe.Items.Clear();
            }

            updateAllowed = true;
        }

        /// <summary>
        /// Select exercise listbox
        /// </summary>
        private void lbExercises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox lbe) return;

            updateAllowed = false;

            int indexClient = lbe.Name[^1] - '0';

            if (indexClient >= currentClients.Count)
            {
                updateAllowed = true;

                return;
            }

            var lb = (ListBox)FindName($"lbOrders_{indexClient}");

            if (currentClients[indexClient][lb.SelectedIndex] is OrderRegular order)
            {
                var rtb = (RichTextBox)FindName($"rtbExercise_{indexClient}");

                rtb.Document = order[lbe.SelectedIndex].ToFlowDocument();
            }

            updateAllowed = true;
        }

        /// <summary>
        /// Open folder in browser button
        /// </summary>
        private void butClientOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            string path = currentClients[indexClient].FolderPath;

            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
            }
            else
            {
                InfoBox infoBox = new("Папка не существует. Проверьте информацию о клиенте.");
                infoBox.ShowDialog();
            }

            updateAllowed = true;
        }

        /// <summary>
        /// Add single order button
        /// </summary>
        private void butClientAddOrderSingle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            WindowAddOrderSingle window = new(currentClients[indexClient]);
            window.ShowDialog();

            updateAllowed = true;

            if (window.DialogResult == true)
            {
                UpdateContent();
            }
        }

        /// <summary>
        /// Add regular order button
        /// </summary>
        private void butClientAddOrderRegular_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            WindowAddOrderRegular window = new(currentClients[indexClient]);
            window.ShowDialog();

            updateAllowed = true;

            if (window.DialogResult == true)
            {
                UpdateContent();
            }
        }

        /// <summary>
        /// Edit order button
        /// </summary>
        private void butOrderEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            var lb = (ListBox)FindName($"lbOrders_{indexClient}");

            if (lb.Items.Count < 1)
            {
                updateAllowed = true;

                return;
            }

            Order order = currentClients[indexClient][lb.SelectedIndex];

            if (order is OrderSingle orderSingle)
            {
                WindowAddOrderSingle window = new(currentClients[indexClient], true, orderSingle);
                window.ShowDialog();

                updateAllowed = true;

                if (window.DialogResult == true)
                {
                    UpdateContent();
                }
            }
            else if (order is OrderRegular orderRegular)
            {
                WindowAddOrderRegular window = new(currentClients[indexClient], true, orderRegular);
                window.ShowDialog();

                updateAllowed = true;

                if (window.DialogResult == true)
                {
                    UpdateContent();
                }
            }

            // Not required
            updateAllowed = true;
        }

        /// <summary>
        /// Add exercise button
        /// </summary>
        private void butAddExercise_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            var lb = (ListBox)FindName($"lbOrders_{indexClient}");

            if (lb.Items.Count < 1)
            {
                updateAllowed = true;

                return;
            }

            Order order = currentClients[indexClient][lb.SelectedIndex];

            if (order is OrderRegular orderRegular)
            {
                WindowAddExercise window = new(orderRegular);
                window.ShowDialog();

                updateAllowed = true;

                if (window.DialogResult == true)
                {
                    UpdateContent();
                }
            }
            else
            {
                updateAllowed = true;
            }
        }

        /// <summary>
        /// Edit exercise button
        /// </summary>
        private void butExerciseEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            updateAllowed = false;

            int indexClient = button.Name[^1] - '0';

            if (indexClient >= currentClients.Count) return;

            var lb = (ListBox)FindName($"lbOrders_{indexClient}");

            if (lb.Items.Count < 1)
            {
                updateAllowed = true;

                return;
            }

            Order order = currentClients[indexClient][lb.SelectedIndex];

            if (order is OrderRegular orderRegular)
            {
                var lbe = (ListBox)FindName($"lbExercises_{indexClient}");

                if (lbe.Items.Count < 1)
                {
                    updateAllowed = true;

                    return;
                }

                WindowAddExercise window = new(orderRegular, true, orderRegular[lbe.SelectedIndex]);
                window.ShowDialog();

                updateAllowed = true;

                if (window.DialogResult == true)
                {
                    UpdateContent();
                }
            }
            else
            {
                updateAllowed = true;
            }
        }

        /// <summary>
        /// Edit selected event
        /// </summary>
        private void buttonEditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEvent == null) return;

            updateAllowed = false;

            WindowAddEvent window = new(globalVars, true, selectedEvent);
            window.ShowDialog();

            updateAllowed = true;

            if (window.DialogResult == true)
            {
                UpdateContent();
            }
        }

        /// <summary>
        /// Week events list box selection changed
        /// </summary>
        private void lbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!updateAllowed) return;

            if (sender is not ListBox lb) return;

            updateAllowed = false;

            int indexLB = int.Parse(lb.Name.Substring(lb.Name.Length - 2, 2));

            ClearLBSelection(indexLB);

            if (currentEvents[indexLB][lb.SelectedIndex].Event != null)
            {
                selectedEvent = currentEvents[indexLB][lb.SelectedIndex].Event;

                buttonEditEvent.IsEnabled = true;
            }
            else
            {
                selectedEvent = null;

                buttonEditEvent.IsEnabled = false;
            }

            updateAllowed = true;
        }

        /// <summary>
        /// CLear week listbox selections
        /// </summary>
        private void ClearLBSelection(int indexSkip = -1)
        {
            for (int i = 0; i < 14; i++)
            {
                if (i != indexSkip)
                {
                    var lbi = (ListBox)FindName($"lbData{i:D2}");

                    lbi.SelectedIndex = -1;
                }
            }
        }
    }
}
