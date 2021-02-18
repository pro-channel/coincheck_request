using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CoincheckRequest
{

    /// <summary>
    /// coinche
    /// </summary>
    public class CoincheckFunctions : AbstractFunctions
    {

        /// <summary>
        /// アクセスキー
        /// </summary>
        private static string accessKey;

        /// <summary>
        /// シークレットキー
        /// </summary>
        private static string secretKey;

        /// <summary>
        /// Uri
        /// </summary>
        private static readonly string baseUri = "https://coincheck.com";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CoincheckFunctions()
        {
            // 設定読み込み
            accessKey = CoincheckRequest.Properties.Settings.Default["AccessKey"].ToString();
            secretKey = CoincheckRequest.Properties.Settings.Default["SecretKey"].ToString();
        }


        #region Request処理関連

        /// <summary>
        /// リクエスト処理
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IRestResponse Request(Method method, string path, string param = "")
        {
            var url = baseUri + path;
            var nonce = GetNonce;
            var message = Convert.ToString(nonce) + url + param;
            var signature = GenerateNewHmac(secretKey, message);
            var client = new RestClient(url);
            var request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("access-signature", signature);
            request.AddHeader("access-nonce", Convert.ToString(nonce));
            request.AddHeader("access-key", accessKey);
            request.AddHeader("content-type", "application/json");
            if (!string.IsNullOrEmpty(param))
            {
                request.AddParameter("application/json", param, ParameterType.RequestBody);
            }

            var response = client.Execute(request);
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                throw new Exception();
            }
            Console.WriteLine(response.Content);
            return response;
        }

        /// <summary>
        /// タイムスタンプ取得
        /// </summary>
        public static long GetNonce
        {
            get
            {
                var jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return (long)((DateTime.UtcNow - jan1St1970).TotalMilliseconds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="challenge"></param>
        /// <returns></returns>
        public static string GenerateNewHmac(string password, string challenge)
        {
            var hmc = new HMACSHA256(Encoding.ASCII.GetBytes(password));
            var hmres = hmc.ComputeHash(Encoding.ASCII.GetBytes(challenge));
            return BitConverter.ToString(hmres).Replace("-", "").ToLower();
        }
        #endregion

        #region Response取得処理
        /// <summary>
        /// responseのJson取得
        /// </summary>
        /// <param name="type">リクエストの種類</param>
        /// <param name="param">POST or GET パラメータ</param>
        /// <returns></returns>
        public static JObject GetJObject(RequestEnum type, string param = "")
        {
            try
            {
                IRestResponse response = Request(type.GetMethod(), type.GetPath(), param);
                JObject jObj = JObject.Parse(response.Content);
                if ((type != RequestEnum.Board && type != RequestEnum.Ticker && type != RequestEnum.Balance) && !System.Convert.ToBoolean(jObj["success"].ToString()))
                {
                    throw new Exception();
                }

                return jObj;
            }
            catch (Exception e)
            {
                throw new Exception(type.GetActionName() + "リクエストでエラーが発生しました。"  + e.StackTrace);
            }
        }


        /// <summary>
        /// 注文キャンセル
        /// 戻り値:端数売が発生
        /// </summary>
        /// <param name="orderId">注文ID</param>
        public override void RequestCancel(string orderId)
        {
            try
            {
                IRestResponse response = Request(RequestEnum.ExchangeCanxel.GetMethod(), RequestEnum.ExchangeCanxel.GetPath() + orderId.ToString());
                JObject jObj = JObject.Parse(response.Content);
                if (!System.Convert.ToBoolean(jObj["success"].ToString()))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception(RequestEnum.ExchangeCanxel.GetActionName() + "リクエストでエラーが発生しました。");
            }
        }

        /// <summary>
        /// 円の残高を取得する
        /// </summary>
        /// <returns>JPY残高</returns>
        public override double GetJpyRemaining()
        {
            try
            {
                JObject jObj = RequestBalance();
                if (!System.Convert.ToBoolean(jObj["success"].ToString()))
                {
                    throw new Exception();
                }
                double btc = (double)jObj["jpy"];
                return btc;
            }
            catch (Exception)
            {
                throw new Exception(RequestEnum.Balance.GetActionName() + "リクエストでエラーが発生しました。");
            }
        }

        /// <summary>
        /// BTCの残金を取得する
        /// </summary>
        /// <returns>BTC残高</returns>
        public override double GetBtcRemaining()
        {
            try
            {
                JObject jObj = RequestBalance();
                if (!System.Convert.ToBoolean(jObj["success"].ToString()))
                {
                    throw new Exception();
                }
                double btc = (double)jObj["btc"];
                return btc;
            }
            catch (Exception)
            {
                throw new Exception(RequestEnum.Balance.GetActionName() + "リクエストでエラーが発生しました。");
            }
        }

        /// <summary>
        /// 買い注文を出す
        /// </summary>
        /// <param name="rate">レート</param>
        /// <param name="amount">量</param>
        /// <returns>注文ID</returns>
        public override string Buy(int rate, double amount)
        {
            var param = new Dictionary<string, string>
            {
                { "pair", "btc_jpy" },
                { "order_type", "buy" },
                { "rate", rate.ToString() },
                { "amount", amount.ToString() },
            };

            JObject obj = GetJObject(RequestEnum.ExchangeBuyOrder, JsonConvert.SerializeObject(param));
            return obj["id"].ToString();
        }

        /// <summary>
        /// 成行買注文
        /// </summary>
        /// <param name="rate">レート</param>
        /// <param name="amount">量</param>
        /// <returns>注文ID</returns>
        //public override string MarketBuy(int rate, double amount)
        //{
        //    var param = new Dictionary<string, string>
        //    {
        //        { "pair", "btc_jpy" },
        //        { "order_type", "market_buy" },
        //        { "market_buy_amount", Math.Truncate(rate * amount).ToString() }
        //    };

        //    JObject obj = GetJObject(RequestEnum.ExchangeBuyOrder, JsonConvert.SerializeObject(param));
        //    return obj["id"].ToString();
        //}

        /// <summary>
        /// 成行売注文
        /// </summary>
        /// <param name="rate">レート</param>
        /// <param name="amount">量</param>
        /// <returns>注文ID</returns>
        //public override string MarketSell(int rate, double amount)
        //{
        //    var param = new Dictionary<string, string>
        //    {
        //        { "pair", "btc_jpy" },
        //        { "order_type", "market_sell" },
        //        { "amount", amount.ToString() }
        //    };

        //    JObject obj = GetJObject(RequestEnum.ExchangeSaleOrder, JsonConvert.SerializeObject(param));
        //    return obj["id"].ToString();
        //}

        /// <summary>
        /// 売り注文を出す
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public override string Sale(int rate, double amount)
        {
            var param = new Dictionary<string, string>
            {
                { "rate", rate.ToString() },
                { "amount", amount.ToString() },
                { "order_type", "sell" },
                { "pair", "btc_jpy" },
            };

            JObject obj = GetJObject(RequestEnum.ExchangeSaleOrder, JsonConvert.SerializeObject(param));
            return obj["id"].ToString();
        }

        /// <summary>
        /// 板情報を取得
        /// </summary>
        /// <returns></returns>
        protected override JObject GetBoardJson()
        {
            return GetJObject(RequestEnum.Board);
        }

        /// <summary>
        /// ティッカー取得
        /// </summary>
        /// <returns></returns>
        public override JObject GetTickerJson()
        {
            return GetJObject(RequestEnum.Ticker);
        }

        /// <summary>
        /// 注文中一覧取得
        /// </summary>
        /// <returns></returns>
        private static JObject RequestExchangeList()
        {
            return GetJObject(RequestEnum.ExchangeList);
        }

        /// <summary>
        /// 取引履歴
        /// </summary>
        /// <returns></returns>
        private static JObject RequestTransactions()
        {
            return GetJObject(RequestEnum.Transactions);
        }

        /// <summary>
        /// 残高取得
        /// </summary>
        /// <returns></returns>
        private static JObject RequestBalance()
        {
            return GetJObject(RequestEnum.Balance);
        }


        #endregion

    }
}
