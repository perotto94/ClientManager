namespace ClientManager
{
    public partial class Client
    {
        /// <summary>
        /// Full name
        /// </summary>
        public string Name
        {
            get
            {
                string result = FirstName;

                if (MiddleName != "")
                {
                    result = result + " " + MiddleName;
                }

                if (LastName != "")
                {
                    result = result + " " + LastName;
                }

                return result;
            }
        }

        /// <summary>
        /// Total value of all orders
        /// </summary>
        public int ValueTotal(bool completedOnly = true)
        {
            int result = 0;

            foreach (Order order in Orders)
            {
                if (order is OrderSingle orderSingle)
                {
                    if (completedOnly && orderSingle.Status != OrderStatus.Completed)
                    {
                        continue;
                    }

                    result += orderSingle.Value;
                }
                else if (order is OrderRegular orderRegular)
                {
                    result += orderRegular.ValueTotal(completedOnly);
                }
            }

            return result;
        }

        /// <summary>
        /// Debt
        /// </summary>
        public int Debt()
        {
            int result = 0;

            foreach (Order order in Orders)
            {
                if (order is OrderSingle orderSingle)
                {
                    if (orderSingle.Status == OrderStatus.AwaitForPayment)
                    {
                        result += orderSingle.Value;
                    }
                }
                else if (order is OrderRegular orderRegular)
                {
                    foreach (Exercise exercise in orderRegular.Exercises)
                    {
                        if (exercise.Status == ExerciseStatus.AwaitForPayment)
                        {
                            result += exercise.Value;
                        }
                    }
                }
            }

            return result;
        }
    }
}
