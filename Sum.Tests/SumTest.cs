using System.Diagnostics;
using Xunit;

namespace Sum.Tests
{
    public class SumTest : BaseTest
    {
        [Theory]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(10_000_000)]
        public void SimpleSum(int countOfElements)
        {
            var stopwatch = new Stopwatch();
            Debug.WriteLine($"Процесс SimpleSum для {countOfElements} запущен");

            var list = GenerateList(countOfElements);

            stopwatch.Start();
            var sum = list.Sum(x => x);
            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс SimpleSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");

            Assert.True(sum > 0);
        }

        [Theory]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(10_000_000)]
        public void TPLSum(int countOfElements)
        {
            var stopwatch = new Stopwatch();
            Debug.WriteLine($"Процесс TPLSum для {countOfElements} запущен");
            const int countOfThreads = 10;
            var list = GenerateList(countOfElements).ToList();
            var sum = 0;

            //Гораздо эффективнее предварительно разбить список на части и найти их суммы по отдельности
            //Но этот метод помимо подготовки ТД также потребует сложения частей после метода TPL, что противоречит условиям задачи

            stopwatch.Start();
            Parallel.For(0, countOfElements, new ParallelOptions()
            {
                MaxDegreeOfParallelism = countOfThreads
            }, (index)=>
            {
                Interlocked.Add(ref sum, list[index]);
            });

            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс TPLSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");

            Assert.True(sum > 0);
        }

        [Theory]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(10_000_000)]
        public void LinqSum(int countOfElements)
        {
            var stopwatch = new Stopwatch();
            Debug.WriteLine($"Процесс LinqSum для {countOfElements} запущен");
            const int countOfThreads = 10;
            var list = GenerateList(countOfElements).ToList();

            stopwatch.Start();
            var sum = list
                .AsParallel()
                .WithDegreeOfParallelism(countOfThreads)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Sum();
          
            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс LinqSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");

            Assert.True(sum > 0);
        }
    }
}