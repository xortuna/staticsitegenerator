using StaticSiteGenerator.Engine;

namespace StaticSiteTests
{
    [TestClass]
    public class ImageOptimiserTests
    {

        [TestMethod]
        public void TestOptimisation()
        {
            var rootDirectory = new DirectoryInfo("C:\\Users\\Jack\\source\\repos\\StaticSkiSite");
            ImagePixelDensityAutomator iOpti = new ImagePixelDensityAutomator(rootDirectory, true);

            string text = "![float-end inline-image][Pelozet Piste](/assets/image/blog/topskirunsmenuires/1_map.png)";
            var result = iOpti.CompileText(text);
            Assert.AreEqual("![float-end inline-image][Pelozet Piste](/assets/image/blog/topskirunsmenuires/1_map.png)(/assets/image/blog/topskirunsmenuires/1_map.png 1x, /assets/image/blog/topskirunsmenuires/1_map-2x.png 2x)", result);
        }

        [TestMethod]
        public void TestWebPUpgrade()
        {
            var rootDirectory = new DirectoryInfo("C:\\Users\\Jack\\source\\repos\\StaticSkiSite");
            ImagePixelDensityAutomator iOpti = new ImagePixelDensityAutomator(rootDirectory, true);

            string text = "![Pelozet Piste](/assets/image/blog/topskiruntignes/1_t.png)";
            var result = iOpti.CompileText(text);
            Assert.AreEqual("![Pelozet Piste](/assets/image/blog/topskiruntignes/1_t.png)(/assets/image/blog/topskiruntignes/1_t.webp 1x)", result);
        }
    }
}