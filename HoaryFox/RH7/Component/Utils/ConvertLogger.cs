using System;
using System.IO;
using System.Text;

namespace HoaryFox.Component.Utils
{
    public class ConvertLogger
    {
        private readonly StringBuilder _logger = new StringBuilder();
        private readonly string _path;

        public ConvertLogger(string path, string version)
        {
            _path = path;
            _logger.AppendLine(@"--------------------------------------");
            _logger.AppendLine(@" ____  ____                                       ________");
            _logger.AppendLine(@"|_   ||   _|                                     |_   __  |");
            _logger.AppendLine(@"  | |__| |     .--.    ,--.    _ .--.    _   __    | |_ \_|   .--.    _   __");
            _logger.AppendLine(@"  |  __  |   / .'`\ \ `'_\ :  [ `/'`\]  [ \ [  ]   |  _|    / .'`\ \ [ \ [  ]");
            _logger.AppendLine(@" _| |  | |_  | \__. | // | |,  | |       \ '/ /   _| |_     | \__. |  > '  <");
            _logger.AppendLine(@"|____||____|  '.__.'  \'-;__/ [___]    [\_:  /   |_____|     '.__.'  [__]`\_]");
            _logger.AppendLine(@"                                        \__.'");
            _logger.AppendLine($"                                                            version:{version}");
            _logger.AppendLine(@"  ST-Bridge to Brep Convert Log");
            _logger.AppendLine(@"--------------------------------------");
            _logger.AppendLine($"::INFO   :: 変換開始 | {DateTime.Now}");
        }

        public void Clear()
        {
            _logger.Clear();
        }

        public void AppendInfoMessage(string message)
        {
            _logger.AppendLine($"::INFO   :: {message}");
        }

        public void AppendInfoConvertStartMessage(string message)
        {
            _logger.AppendLine("--------------------------------------");
            _logger.AppendLine($"::INFO   :: {message}の変換を開始しました。 | {DateTime.Now}");
        }

        public void AppendInfoConvertEndMessage(string message)
        {
            _logger.AppendLine($"::INFO   :: {message}の変換を終了しました。 | {DateTime.Now}");
            _logger.AppendLine("--------------------------------------");
        }

        public void AppendInfoDataNotFoundMessage(string message)
        {
            _logger.AppendLine($"::INFO   :: {message}のデータはありませんでした。 | {DateTime.Now}");
            _logger.AppendLine("--------------------------------------");
        }

        public void AppendInfo(string guid, string message)
        {
            _logger.AppendLine($"::INFO   :: [{guid}] | {message}");
        }

        public void AppendConvertSuccess(string guid)
        {
            _logger.AppendLine($"::INFO   :: [{guid}] | 変換完了");
        }

        public void AppendWarning(string guid, string message)
        {
            _logger.AppendLine($"::WARNING:: [{guid}] | {message}");
        }

        public void AppendConvertWarning(string guid, string message)
        {
            _logger.AppendLine($"::WARNING:: [{guid}] | 変換結果 要確認 | {message}");
        }

        public void AppendError(string guid, string message)
        {
            _logger.AppendLine($"::ERROR  :: [{guid}] | {message}");
        }

        public void AppendConvertFailed(string guid, string message)
        {
            _logger.AppendLine($"::ERROR  :: [{guid}] | 変換失敗 | {message}");
        }

        public void AppendSummary(int[] resultCount)
        {
            _logger.AppendLine($"::INFO   :: [SUMMARY] | {resultCount[0]} 件の変換に成功しました。");
            _logger.AppendLine($"::INFO   :: [SUMMARY] | {resultCount[1]} 件が変換出来ましたが、結果の確認が必要です。");
            _logger.AppendLine($"::INFO   :: [SUMMARY] | {resultCount[2]} 件の変換に失敗しました。");
        }

        public void Serialize()
        {
            AppendInfoConvertEndMessage("ST-BridgeデータのBrepへ");
            File.WriteAllText(_path + "/S2B_convert.log", _logger.ToString());
        }
    }
}
