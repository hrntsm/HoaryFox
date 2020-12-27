using Karamba.Models;
using STBDotNet.Elements;

namespace karambaConnect.K2S
{
    public static class StbElement
    {
        public static StbElements GetData(Model model)
        {
            var elem = new StbElements
            {
                Version = "1.4.00",
                Common = StbCommon.Set(),
                Model = StbModel.Set(model)
            };
            return elem;
        }
    }
}