using Intems.Devices;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DataRetrieverTest
    {
        private byte[] _data;
        private void OnPackageRetrieved(object sender, PackageDataArgs args)
        {
            _data = args.Data;
        }

        [TearDown]
        public void Cleanup()
        {
            _data = null;
        }

        [Test]
        public void OneGoodPackage()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg = new byte[]{0x7e, 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f};
            retriever.AddBytes(pkg);
            var expected = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35 };

            CollectionAssert.AreEquivalent(expected, _data);
        }

        [Test]
        public void OneGoodAndTrimmedPackage()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg = new byte[] { //first package
                                   0x7e, 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f, 
                                   //second package
                                   0x7e, 0x01, 0x02, 0x01, 0x00 };
            retriever.AddBytes(pkg);
            var expected = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35 };

            CollectionAssert.AreEquivalent(expected, _data);
        }
    }
}
