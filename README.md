# PropertyChangedEventArgs.PropertyName をスタックしてみるテスト

## 発端

https://twitter.com/ruche7/status/1210646363068780544

## 結果

* ThreadLocal を使うと ConcurrentDictionary でキャッシュする場合と大差無い速度。(遅い！)
* ThreadLocal 無しなら毎回 PropertyChangedEventArgs を new するのとほぼ同じ速度。(早い！)

## 結論

SynchronizationContext を使うなどして必ず単一スレッド(WPFならUIスレッド)でイベントを処理する前提ならアリかも？
