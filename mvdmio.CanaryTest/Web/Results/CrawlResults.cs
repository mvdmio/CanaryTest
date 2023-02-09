using System.Collections.Concurrent;

namespace mvdmio.CanaryTest.Web.Results;

public class CrawlResults
{
   private readonly ConcurrentDictionary<string, CheckResult> _checkResults;

   public IEnumerable<CheckResult> Checks => _checkResults.Values;
   public IEnumerable<CheckResult> Failed => _checkResults.Values.Where(x => x.IsSuccess == false);
   public IEnumerable<CheckResult> Succeeded => _checkResults.Values.Where(x => x.IsSuccess == true);

   public CrawlResults()
   {
      _checkResults = new ConcurrentDictionary<string, CheckResult>();
   }

   public void Clear()
   {
      _checkResults.Clear();
   }

   public bool Contains(Uri url)
   {
      return _checkResults.ContainsKey(url.AbsoluteUri);
   }

   public CheckResult AddResult(Uri url)
   {
      var newResult = new CheckResult {
         Url = url
      };

      if (_checkResults.TryAdd(url.AbsoluteUri, newResult))
         return newResult;

      return _checkResults[url.AbsoluteUri];
   }
}