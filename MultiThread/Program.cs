using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThread
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var StartDate=DateTime.Now;

            #region classic thread
            //Thread thr1 = new Thread(() => Islem1());
            //Thread thr2 = new Thread(Islem2);

            //thr1.Start();
            //thr2.Start();

            //thr1.Join();
            //thr2.Join();

            //var EndDate = DateTime.Now;

            //var fark = EndDate - StartDate;

            //Console.WriteLine("Toplam süre:" + fark.TotalSeconds);
            #endregion

            #region samByTAsks

        //    Task t1 = Islem1();
            Task t2 = Islem2();
           

        //    t1.Wait();
            t2.Wait();

            var EndDate = DateTime.Now;
            var fark = EndDate - StartDate;
            Console.WriteLine("Toplam süre:" + fark.TotalSeconds);


            #endregion

            // Task m1Task= Method1();
            //Task<int> m2Task = Method2();

            //Console.WriteLine("before wait:" + m2Task.Result);

            ////  m1Task.Wait();
            //m2Task.Wait();

            //Console.WriteLine("after wait:"+m2Task.Result);


            //Console.WriteLine("END OF FILE");
            //Console.ReadKey();

        }

        public static async Task Method1()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(" Method 1");
                    // Do something
                    Task.Delay(100).Wait();
                }
            });
        }


        public static async Task<int> Method2()
        {
            int sum = 0;
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine(" Method 2");
                sum += i;
                // Do something
                Task.Delay(100).Wait();
            }
            return sum;
        }

        static async Task Islem1()
        {
            await Task.Run(() =>
            {
                Task.Delay(5000).Wait();
            });
        }

        static async Task Islem2()
        {
             await Task.Run(() =>
              {
                  Task.Delay(5000).Wait();
                  Console.WriteLine("birinci task");
              });

         //   return await Task.FromResult<int>(0);
        }
    }
}
