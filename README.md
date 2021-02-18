
# coincheck_request


## 機能概要
coincheckのAPIへのリクエスト処理を行うプログラムです。
実行可能なAPIの処理は「実装処理」を確認してください。



## 設定
「プロジェクト右クリック」→「設定」でAccessKey,SecretKeyを設定してください。
coincheckのプライベートAPIを実行する場合この2つが設定されていないと実行できません。
app.configを直接編集でもOKです。
この二つのキーは↓で取得できます。
<https://coincheck.com/ja/api_settings>



## 依存関係
このプログラムではAPIリクエストのJSON操作にNewtonsoft.jsonを使用しています。
tickerの取得等の処理でNewtonsoft.jsonのオブジェクトが戻り値に設定されています。
呼び出しするプロジェクトではNewtonsoft.jsonの参照追加が必要です。
Nugetでインストールできます。

## 注意事項
coincheckの仕様変更が発生した場合、正常動作しない可能性があります。


## 実装処理

### RequestCancel
注文キャンセル処理です。キャンセル対象の注文IDを渡してください。

### GetJpyRemaining
JPYの残高を取得します。

### GetBtcRemaining
BTCの残高を取得します。

### Buy
買い注文リクエストを行います。レートを注文量を指定してください。

### Sale
売り注文リクエストを行います。レートを注文量を指定してください。

### GetBoardJson
板情報を取得します。戻り値のJObjectはAPIのJSON構造のままです。
↓に板情報のJSON使用があります。
<https://coincheck.com/ja/documents/exchange/api#order-book>

### GetTickerJson
ティッカー情報を取得します。戻り値のJObjectはAPIのJSON構造のままです。
↓に板情報のJSON使用があります。
<https://coincheck.com/ja/documents/exchange/api#ticker>

### RequestExchangeList
注文中一覧を取得します。
<https://coincheck.com/ja/documents/exchange/api#order-opens>

### RequestExchangeList
取引履歴を取得します。
<https://coincheck.com/ja/documents/exchange/api#order-transactions>
