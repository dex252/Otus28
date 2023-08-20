namespace Sum.Tests
{
    abstract public class BaseTest
    {
        private Random Random { get; set; }
        public BaseTest()
        {
            Random = new Random();
        }

        protected IEnumerable<int> GenerateList(int count)
        {
            var list = new List<int>(count);
            
            for (int i = 0; i < count; i++)
            {
                var element = Random.Next(0, 100);
                list.Add(element);
            }

            return list;
        }
    }
}
