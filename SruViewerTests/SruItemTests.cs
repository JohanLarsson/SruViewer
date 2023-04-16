namespace SruViewerTests
{
    using System;
    using NUnit.Framework;
    using SruViewer;

    public static class SruItemTests
    {
        [Test]
        public static void ReadWinAndLoss()
        {
            var sru = """
                #BLANKETT K4-2022P4
                #IDENTITET 198001025228 20230415 085031
                #UPPGIFT 3100 35
                #UPPGIFT 3101 AA
                #UPPGIFT 3102 24385
                #UPPGIFT 3103 17921
                #UPPGIFT 3104 6464
                #UPPGIFT 3105 0
                #UPPGIFT 3100 303
                #UPPGIFT 3101 ACET
                #UPPGIFT 3102 53918
                #UPPGIFT 3103 54268
                #UPPGIFT 3104 0
                #UPPGIFT 3105 350
                #BLANKETTSLUT
                #FIL_SLUT

                """.AsSpan();
            var win = SruItem.Read(ref sru);
            Assert.AreEqual(35, win!.Quantity);
            Assert.AreEqual("AA", win.Symbol);
            Assert.AreEqual(24385, win.Proceeds);
            Assert.AreEqual(17921, win.Basis);
            Assert.AreEqual(6464, win.Win);
            Assert.AreEqual(0, win.Loss);

            var loss = SruItem.Read(ref sru);
            Assert.AreEqual(303, loss!.Quantity);
            Assert.AreEqual("ACET", loss.Symbol);
            Assert.AreEqual(53918, loss.Proceeds);
            Assert.AreEqual(54268, loss.Basis);
            Assert.AreEqual(0, loss.Win);
            Assert.AreEqual(350, loss.Loss);

            Assert.AreEqual(null, SruItem.Read(ref sru));
            Assert.AreEqual(true, sru.IsEmpty);
        }

        [Test]
        public static void ReadWinAndLossSeparateForms()
        {
            var sru = """
                #BLANKETT K4-2022P4
                #IDENTITET 198001025228 20230415 085031
                #UPPGIFT 3100 35
                #UPPGIFT 3101 AA
                #UPPGIFT 3102 24385
                #UPPGIFT 3103 17921
                #UPPGIFT 3104 6464
                #UPPGIFT 3105 0
                #BLANKETTSLUT
                #BLANKETT K4-2022P4
                #IDENTITET 197805039034 20230415 085031
                #UPPGIFT 3100 303
                #UPPGIFT 3101 ACET
                #UPPGIFT 3102 53918
                #UPPGIFT 3103 54268
                #UPPGIFT 3104 0
                #UPPGIFT 3105 350
                #BLANKETTSLUT
                #FIL_SLUT

                """.AsSpan();
            var win = SruItem.Read(ref sru);
            Assert.AreEqual(35, win!.Quantity);
            Assert.AreEqual("AA", win.Symbol);
            Assert.AreEqual(24385, win.Proceeds);
            Assert.AreEqual(17921, win.Basis);
            Assert.AreEqual(6464, win.Win);
            Assert.AreEqual(0, win.Loss);

            var loss = SruItem.Read(ref sru);
            Assert.AreEqual(303, loss!.Quantity);
            Assert.AreEqual("ACET", loss.Symbol);
            Assert.AreEqual(53918, loss.Proceeds);
            Assert.AreEqual(54268, loss.Basis);
            Assert.AreEqual(0, loss.Win);
            Assert.AreEqual(350, loss.Loss);

            Assert.AreEqual(null, SruItem.Read(ref sru));
            Assert.AreEqual(true, sru.IsEmpty);
        }
    }
}
