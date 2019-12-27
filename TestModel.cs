using System.ComponentModel;

namespace TestApp
{
    /// <summary>
    /// テストモデルインタフェース。
    /// </summary>
    internal interface ITestModel : INotifyPropertyChanged
    {
        int X { get; set; }
        int Y { get; set; }
        int Z { get; set; }
    }

    /// <summary>
    /// StandardBindableBase クラスを継承するテストモデルクラス。
    /// </summary>
    internal class StandardTestModel : StandardBindableBase, ITestModel
    {
        public int X
        {
            get => this.x;
            set => this.SetProperty(ref this.x, value);
        }
        private int x = 0;

        public int Y
        {
            get => this.y;
            set => this.SetProperty(ref this.y, value);
        }
        private int y = 0;

        public int Z
        {
            get => this.z;
            set => this.SetProperty(ref this.z, value);
        }
        private int z = 0;
    }

    /// <summary>
    /// CachedBindableBase クラスを継承するテストモデルクラス。
    /// </summary>
    internal class CachedTestModel : CachedBindableBase, ITestModel
    {
        public int X
        {
            get => this.x;
            set => this.SetProperty(ref this.x, value);
        }
        private int x = 0;

        public int Y
        {
            get => this.y;
            set => this.SetProperty(ref this.y, value);
        }
        private int y = 0;

        public int Z
        {
            get => this.z;
            set => this.SetProperty(ref this.z, value);
        }
        private int z = 0;
    }

    /// <summary>
    /// StackedBindableBase クラスを継承するテストモデルクラス。
    /// </summary>
    internal class StackedTestModel : StackedBindableBase, ITestModel
    {
        public int X
        {
            get => this.x;
            set => this.SetProperty(ref this.x, value);
        }
        private int x = 0;

        public int Y
        {
            get => this.y;
            set => this.SetProperty(ref this.y, value);
        }
        private int y = 0;

        public int Z
        {
            get => this.z;
            set => this.SetProperty(ref this.z, value);
        }
        private int z = 0;
    }
}
