using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;
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
            var sum = 0;
            var list = GenerateList(countOfElements).ToList();

            //Подготовка ТД
            var threads = new Thread[countOfThreads];
            int partSize = (int)Math.Ceiling((double)list.Count / countOfThreads);
            for (int index = 0; index < countOfThreads; index++)
            {
                var start = index * partSize;
                var part = list.GetRange(start, Math.Min(partSize, countOfElements - start));
                var thread = new Thread(() => {
                    Interlocked.Add(ref sum, part.Sum());
                });
                threads[index] = thread;
            }

            Debug.WriteLine($"Процесс TPLSum для {countOfElements} запущен");
            stopwatch.Start();

            Parallel.ForEach(threads, thread => {
                thread.Start();
            });

            foreach (var thread in threads)
            {
                thread.Join();
            }

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

        /// <summary>
        /// Для сравнения
        /// </summary>
        /// <param name="countOfElements"></param>
        [Theory]
    //    [InlineData(100_000)]
    // [InlineData(1_000_000)]
       [InlineData(10_000_000)]
        public void Common(int countOfElements)
        {
            var stopwatch = new Stopwatch();
            const int countOfThreads = 10;
            var list = GenerateList(countOfElements).ToList();

            Debug.WriteLine($"Процесс SimpleSum для {countOfElements} запущен");
            stopwatch.Start();
            var sum = list.Sum(x => x);
            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс SimpleSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");

            stopwatch.Reset();

            sum = 0;
            //Подготовка ТД
            var threads = new Thread[countOfThreads];
            int partSize = (int)Math.Ceiling((double)list.Count / countOfThreads);
            for (int index = 0; index < countOfThreads; index++)
            {
                var start = index * partSize;
                var part = list.GetRange(start, Math.Min(partSize, countOfElements - start));
                var thread = new Thread(() => {
                    Interlocked.Add(ref sum, part.Sum());
                });
                threads[index] = thread;
            }

            Debug.WriteLine($"Процесс TPLSum для {countOfElements} запущен");
            stopwatch.Start();

            Parallel.ForEach(threads, thread => {
                thread.Start(); 
            });

            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();

            elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс TPLSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");
            
            stopwatch.Reset();
            threads = null;

            sum = 0;
            Debug.WriteLine($"Процесс LinqSum для {countOfElements} запущен");
            stopwatch.Start();
            sum = list
                .AsParallel()
                .WithDegreeOfParallelism(countOfThreads)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Sum();

            stopwatch.Stop();

            elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
            Debug.WriteLine($"Процесс LinqSum для {countOfElements} элементов завершен за {elapsedTime} ms, сумма = {sum}");

            stopwatch.Reset();

            Assert.True(sum > 0);
        }
    }
}