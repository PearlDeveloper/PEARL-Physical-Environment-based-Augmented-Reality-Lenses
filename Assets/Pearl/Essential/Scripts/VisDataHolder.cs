using System;

namespace u2vis
{
    /// <summary>
    /// DataProvider that generates some randomized values that can be used for test purposes.
    /// </summary>
    public class VisDataHolder : AbstractDataProvider
    {
        /// <summary>
        /// The DataSet of this data provider.
        /// </summary>
        private DataSet _data = null;
        /// <summary>
        /// Gets the DataSet which this DataProvider provides.
        /// </summary>
        public override DataSet Data => _data;
        /// <summary>
        /// Creates a new instance of the TestDataProvider class.
        /// </summary>
        public VisDataHolder()
        {
            _data = CreateTestData();
        }
        /// <summary>
        /// Creates the radomized test DataSet of this provider.
        /// </summary>
        /// <returns>The resulting DataSet.</returns>
        public DataSet CreateTestData()
        {
            Random r = new Random();
            DataSet data = new DataSet();
            data.Add(new StringDimension("Time Slot", null));
            data.Add(new IntegerDimension("Number Of People Visited", null));

            data[0].Add("Children");
            data[0].Add("Teenagers");
            data[0].Add("Adults");
            data[0].Add("Adults");
            data[0].Add("Adults");
            data[0].Add("Adults");

            int nChildren = r.Next(5);
            int nTeenagers = r.Next(5);
            int nAdults = 10 - nChildren - nTeenagers;

            data[1].Add(nChildren);
            data[1].Add(nTeenagers);
            data[1].Add(nAdults);
            data[1].Add(nChildren);
            data[1].Add(nTeenagers);
            data[1].Add(nAdults);

            return data;
        }
        /// <summary>
        /// Initializes this TestDataProvider by creating the test DataSet.
        /// </summary>
        public void Initialize()
        {
            _data = CreateTestData();
        }
    }
}
