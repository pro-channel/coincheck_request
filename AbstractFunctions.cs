using Newtonsoft.Json.Linq;

namespace CoincheckRequest
{
    public abstract class AbstractFunctions
    {


        /// <summary>
        /// キャンセルリクエスト
        /// </summary>
        /// <param name="orderId"></param>
        public abstract void RequestCancel(string orderId);

        /// <summary>
        /// BTC残高
        /// </summary>
        /// <returns></returns>
        public abstract double GetBtcRemaining();

        /// <summary>
        /// 円残高
        /// </summary>
        public abstract double GetJpyRemaining();

        /// <summary>
        /// 買い注文(成行)
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        //public abstract string MarketBuy(int rate, double amount);

        /// <summary>
        /// 売り注文（成行）
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        //public abstract string MarketSell(int rate, double amount);

        /// <summary>
        /// 売り注文（指値）
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public abstract string Sale(int rate, double amount);

        /// <summary>
        /// 買い注文（指値）
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public abstract string Buy(int rate, double amount);
        
        /// <summary>
        /// 板情報
        /// </summary>
        /// <returns></returns>
        protected abstract JObject GetBoardJson();

        /// <summary>
        /// ティッカー
        /// </summary>
        /// <returns></returns>
        public abstract JObject GetTickerJson();

    }

}
