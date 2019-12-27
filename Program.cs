using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TestApp
{
    internal class Program
    {
#if DEBUG
        private const int BenchmarkLoopCount = 100;
#else
        private const int BenchmarkLoopCount = 10000000;
#endif

        /// <summary>
        /// ベンチマークを実施する。
        /// </summary>
        /// <typeparam name="TModel">テストモデル型。</typeparam>
        /// <param name="name">テスト名。</param>
        /// <remarks>
        /// 実計測は Release ビルドで行うこと。
        /// </remarks>
        private static void DoBenchmark<TModel>(string name)
            where TModel : ITestModel, new()
        {
            var model = new TModel();
            model.PropertyChanged += OnModelPropertyChanged;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchmarkLoopCount; ++i)
            {
                model.X = i + 1;
            }
            sw.Stop();

            Console.WriteLine($@"{name} : {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// テストモデルの PropertyChanged イベントを処理する。
        /// </summary>
        /// <remarks>
        /// Debug ビルドでは正しくイベント通知されているか検証を行う。
        /// </remarks>
        private static void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ITestModel model)
            {
                switch (e.PropertyName)
                {
                case @"X":
                    Debug.WriteLine($@"X = {model.X}");
                    model.Y = model.X + 1;
                    break;

                case @"Y":
                    Debug.Assert(model.Y == model.X + 1);
                    Debug.WriteLine($@"Y = {model.Y}");
                    model.Z = model.Y + 1;
                    break;

                case @"Z":
                    Debug.Assert(model.Z == model.Y + 1);
                    Debug.WriteLine($@"Z = {model.Z}");
                    break;
                }
            }
        }

        private static int Main()
        {
            DoBenchmark<StandardTestModel>(nameof(StandardTestModel));
            DoBenchmark<CachedTestModel>(nameof(CachedTestModel));
            DoBenchmark<StackedTestModel>(nameof(StackedTestModel));
            return 0;
        }
    }
}
