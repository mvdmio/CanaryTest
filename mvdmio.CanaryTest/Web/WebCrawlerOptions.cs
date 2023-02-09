namespace mvdmio.CanaryTest.Web;

public class WebCrawlerOptions
{
   public required IEnumerable<Uri> Uris { get; init; }
   public IEnumerable<string> IgnoreList { get; init; } = Array.Empty<string>();
   
   public IEnumerable<string> AllowedHosts => Uris.Select(x => x.Host);

   public bool IsHostAllowed(Uri uri)
   {
      return AllowedHosts.Any(x => x == uri.Host);
   }

   public bool IsOnIgnoreList(Uri uri)
   {
      return IgnoreList.Any(x => uri.AbsoluteUri.Contains(x, StringComparison.InvariantCultureIgnoreCase));
   }
}