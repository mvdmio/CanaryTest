using Microsoft.Playwright;
using mvdmio.CanaryTest.Web.Interfaces;

namespace mvdmio.CanaryTest.Web.Playwright;

public class PlaywrightWebElement : IWebElement
{
   private readonly ILocator _element;

   public PlaywrightWebElement(ILocator element)
   {
      _element = element;
   }

   public async Task EnterText(string value)
   {
      await _element.FillAsync(value);
   }

   public async Task Click()
   {
      await _element.ClickAsync();
   }

   public async Task<string?> GetAttribute(string name)
   {
      return await _element.GetAttributeAsync(name);
   }
}