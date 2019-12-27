// StackedBindableBase で ThreadLocal を使うなら有効化
//#define USE_THREADLOCAL

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if USE_THREADLOCAL
using System.Threading;
#endif

namespace TestApp
{
    /// <summary>
    /// プロパティ変更通知をサポートするクラスの抽象基底クラス。
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public abstract event PropertyChangedEventHandler? PropertyChanged;

        protected BindableBase() { }

        /// <summary>
        /// プロパティ値を設定し、変更をイベント通知する。
        /// </summary>
        /// <typeparam name="T">プロパティ値の型。</typeparam>
        /// <param name="field">設定先フィールド参照。</param>
        /// <param name="value">設定値。</param>
        /// <param name="propertyName">プロパティ名。</param>
        protected void SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string propertyName = @"")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                this.RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// PropertyChanged イベントを発生させる。
        /// </summary>
        /// <param name="propertyName">プロパティ名。</param>
        protected abstract void RaisePropertyChanged(
            [CallerMemberName] string propertyName = @"");
    }

    /// <summary>
    /// 毎回 PropertyChangedEventArgs を生成する BindableBase 実装クラス。
    /// </summary>
    public abstract class StandardBindableBase : BindableBase
    {
        public override event PropertyChangedEventHandler? PropertyChanged;

        protected override void RaisePropertyChanged(
            [CallerMemberName] string propertyName = "")
            =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// PropertyChangedEventArgs をキャッシュする BindableBase 実装クラス。
    /// </summary>
    public abstract class CachedBindableBase : BindableBase
    {
        public override event PropertyChangedEventHandler? PropertyChanged;

        protected override void RaisePropertyChanged(
            [CallerMemberName] string propertyName = "")
            =>
            this.PropertyChanged?.Invoke(
                this,
                propertyChangedArgCache.GetOrAdd(
                    propertyName,
                    name => new PropertyChangedEventArgs(name)));

        /// <summary>
        /// プロパティ名ごとの PropertyChangedEventArgs キャッシュディクショナリ。
        /// </summary>
        private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs>
        propertyChangedArgCache =
            new ConcurrentDictionary<string, PropertyChangedEventArgs>();
    }

    /// <summary>
    /// PropertyChangedEventArgs を継承して PropertyName プロパティをスタックする
    /// BindableBase 実装クラス。
    /// </summary>
    public abstract class StackedBindableBase : BindableBase
    {
        public override event PropertyChangedEventHandler? PropertyChanged;

        protected override void RaisePropertyChanged(
            [CallerMemberName] string propertyName = "")
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
#if USE_THREADLOCAL
                var arg = propertyChangedArg.Value!;
#else
                var arg = this.propertyChangedArg;
#endif
                arg.Push(propertyName);
                try
                {
                    handler.Invoke(this, arg);
                }
                finally
                {
                    arg.Pop();
                }
            }
        }

        /// <summary>
        /// プロパティ名をスタックする PropertyChangedEventArgs 継承クラス。
        /// </summary>
        private sealed class StackedPropertyChangedEventArgs : PropertyChangedEventArgs
        {
            public StackedPropertyChangedEventArgs() : base("") { }

            /// <summary>
            /// スタックの先頭からプロパティ名を取得する。
            /// </summary>
            public override string PropertyName => this.propertyNameStack.Peek();

            /// <summary>
            /// スタックの先頭にプロパティ名を追加する。
            /// </summary>
            /// <param name="propertyName">プロパティ名。</param>
            public void Push(string propertyName) =>
                this.propertyNameStack.Push(propertyName);

            /// <summary>
            /// スタックの先頭からプロパティ名を削除する。
            /// </summary>
            public void Pop() => this.propertyNameStack.Pop();

            /// <summary>
            /// プロパティ名スタック。
            /// </summary>
            private readonly Stack<string> propertyNameStack = new Stack<string>();
        }

#if USE_THREADLOCAL
        /// <summary>
        /// スレッド別の StackedPropertyChangedEventArgs オブジェクト。
        /// </summary>
        private static readonly ThreadLocal<StackedPropertyChangedEventArgs>
        propertyChangedArg =
            new ThreadLocal<StackedPropertyChangedEventArgs>(
                () => new StackedPropertyChangedEventArgs());
#else
        /// <summary>
        /// StackedPropertyChangedEventArgs オブジェクト。
        /// </summary>
        private readonly StackedPropertyChangedEventArgs propertyChangedArg =
            new StackedPropertyChangedEventArgs();
#endif
    }
}
