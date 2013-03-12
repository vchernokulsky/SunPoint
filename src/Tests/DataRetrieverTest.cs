using System.Collections.Generic;
using Intems.Devices;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DataRetrieverTest
    {
        private IList<byte[]> _dataList;

        private void OnPackageRetrieved(object sender, PackageDataArgs args)
        {
            _dataList.Add(args.Data);
        }

        [SetUp]
        public void Initialization()
        {
            _dataList = new List<byte[]>();
        }

        [TearDown]
        public void Cleanup()
        {
            _dataList = null;
        }

        [Test]
        public void OneGoodPackage()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg = new byte[] {0x7e, 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f};
            retriever.AddBytes(pkg);

            var expected = new byte[] {0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35};
            Assert.AreEqual(1, _dataList.Count);
            CollectionAssert.AreEquivalent(expected, _dataList[0]);
        }

        [Test]
        public void OneGoodAndTrimmedPackage()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg = new byte[]
                          {
                              //first package
                              0x7e, 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f,
                              //second package
                              0x7e, 0x01, 0x02, 0x01, 0x00
                          };
            retriever.AddBytes(pkg);

            var expected = new byte[] {0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35};
            Assert.AreEqual(1, _dataList.Count);
            CollectionAssert.AreEquivalent(expected, _dataList[0]);
        }

        [Test]
        public void OneFragmentedPackage()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg1 = new byte[] { 0x7e, 0x01, 0x02, 0x01, 0x00, 0x00 };
            var pkg2 = new byte[] { 0x00, 0x3c, 0x70, 0x35, 0x7f };

            retriever.AddBytes(pkg1);
            retriever.AddBytes(pkg2);

            var expected = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35 };
            Assert.AreEqual(1, _dataList.Count);
            CollectionAssert.AreEquivalent(expected, _dataList[0]);
        }

        [Test]
        public void TwoFragmentedPackages()
        {
            var retriever = new DataRetriever();
            retriever.PackageRetrieved += OnPackageRetrieved;

            var pkg1 = new byte[]
                           {
                               //first package
                               0x7e, 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f,
                               //second package
                               0x7e, 0x01, 0x02, 0x01, 0x00
                           };
            var pkg2 = new byte[] { 0x00, 0x00, 0x3c, 0x70, 0x35, 0x7f };

            retriever.AddBytes(pkg1);
            retriever.AddBytes(pkg2);

            var expected1 = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35 };
            var expected2 = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, 0x70, 0x35 };

            Assert.AreEqual(2, _dataList.Count);
            CollectionAssert.AreEquivalent(expected1, _dataList[0]);
            CollectionAssert.AreEquivalent(expected2, _dataList[1]);
        }
    }
}
