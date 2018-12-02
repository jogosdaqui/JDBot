using System.Linq;
using JDBot.Infrastructure.Texts;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Texts
{
    [TestFixture]
    public class RegexFileTest
    {
        private RegexFile _target;

        [OneTimeSetUp]
        public void Setup()
        {
            _target = new RegexFile("Infrastructure/Texts/sample-regex-file-1.json");
            Assert.IsNotNull(_target);
            Assert.AreEqual(3, _target.RegexesCount);
        }

        [Test]
        public void Match_File_Matches()
        {
            // First regex.
            var match = _target.Match("1", "abc__");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("abc", match.ToString());

            // Second regex.
            match = _target.Match("1", "def__");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("def", match.ToString());

            match = _target.Match("1", "deF__");
            Assert.False(match.Success);

            match = _target.Match("1", "__dexptof__");
            Assert.False(match.Success);

            // Thrid regex.
            match = _target.Match("1", "__abc__def__ghi__klm");
            Assert.IsTrue(match.Success);
            Assert.AreEqual("abc__def__ghi__klm", match.ToString());
            Assert.AreEqual("ghi", match.Groups["name"].Value);

            match = _target.Match("1", "__abc__def__ghi__");
            Assert.False(match.Success);
        }

        [Test]
        public void MatchByTag_Tag_OnlyTagMatches()
        {
            var match = _target.MatchByTag("tag1", "1", "def__");
            Assert.IsFalse(match.Success);

            match = _target.MatchByTag("tag1", "1", "aBc___");
            Assert.IsTrue(match.Success);

            match = _target.MatchByTag("tag2", "1", "aBc___");
            Assert.IsTrue(match.Success);

            match = _target.MatchByTag("tag3", "aBc___");
            Assert.IsFalse(match.Success);

            match = _target.MatchByTag("tag1", "__abc__def__ghi__klm");
            Assert.IsTrue(match.Success);

            match = _target.MatchByTag("tag2", "__abc__def__ghi__klm");
            Assert.IsFalse(match.Success);

            match = _target.MatchByTag("tag3", "__abc__def__ghi__klm");
            Assert.IsTrue(match.Success);

            match = _target.MatchByTag("tag4", "__abc__def__ghi__klm");
            Assert.IsFalse(match.Success);
        }

        [Test]
        public void GetResponses_File_Responses()
        {
            // First regex.
            var actual = _target.GetResponses("1", "abc__").ToArray();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("111", actual[0]);
            Assert.AreEqual("222", actual[1]);

            // Second regex.
            actual = _target.GetResponses("1", "def__").ToArray();
            Assert.AreEqual(0, actual.Length);

            actual = _target.GetResponses("1", "deF__").ToArray();
            Assert.AreEqual(0, actual.Length);

            actual = _target.GetResponses("1", "__dexptof__").ToArray();
            Assert.AreEqual(0, actual.Length);

            // Thrid regex.
            actual = _target.GetResponses("1", "__abc__def__ghi__klm").ToArray();
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("333", actual[0]);
            Assert.AreEqual("444", actual[1]);
            Assert.AreEqual("ghi test", actual[2]);

            actual = _target.GetResponses("1", "__abc__def__ghi__").ToArray();
            Assert.AreEqual(0, actual.Length);

            actual = _target.GetResponses("1", "abc__def__ghi__klm").ToArray();
            Assert.AreEqual(5, actual.Length);
            Assert.AreEqual("111", actual[0]);
            Assert.AreEqual("222", actual[1]);
            Assert.AreEqual("333", actual[2]);
            Assert.AreEqual("444", actual[3]);
            Assert.AreEqual("ghi test", actual[4]);
        }

    }
}
