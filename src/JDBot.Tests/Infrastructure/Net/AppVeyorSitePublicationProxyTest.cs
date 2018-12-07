using System;
using System.Threading;
using System.Threading.Tasks;
using JDBot.Domain.Sites;
using JDBot.Infrastructure.Net;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Net
{
    [TestFixture]
    [Ignore("Rodar somente quando necessário, muito demorado e acaba fazendo builds no AppVeyor.")]
    public class AppVeyorSitePublicationProxyTest
    {
        private AppVeyorSitePublicationProxy _target;

        [OneTimeSetUp]
        public void Setup()
        {
            var apiKey = Environment.GetEnvironmentVariable("APPVEYOR_APIKEY");
            _target = new AppVeyorSitePublicationProxy(apiKey);
        }

        [Test]
        public async Task GetPublicationStatusAsync_NoArgs_AppVeyorMasterBuildStatus()
        {
            var actual = await _target.GetPublicationStatusAsync();
            Assert.AreEqual(PublicationStatus.Success, actual);
        }

        [Test]
        public async Task PublishAsync_ApiKey_AppVeyorBuildStarted()
        {
            await _target.PublishAsync();
            var status = await _target.GetPublicationStatusAsync();
            Assert.AreEqual(PublicationStatus.Queued, status);

            do
            {
                status = await _target.GetPublicationStatusAsync();
                Thread.Sleep(30000);
            } while (status != PublicationStatus.Running && status != PublicationStatus.Failed);

            do
            {
                status = await _target.GetPublicationStatusAsync();
                Thread.Sleep(30000);
            } while (status != PublicationStatus.Success && status != PublicationStatus.Failed);

            Assert.AreEqual(PublicationStatus.Success, status);
        }
    }
}
