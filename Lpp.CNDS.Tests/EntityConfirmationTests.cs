using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Lpp.CNDS.Tests
{
    [TestClass]
    public class EntityConfirmationTests
    {
        [TestMethod]
        public void DomainsList()
        {
            var controller = new Api.Domains.DomainController();
            var listAll = controller.List();
            var listOrg = controller.List().Where(x => x.EntityType == DTO.Enums.EntityType.Organization);
            var listDS = controller.List().Where(x => x.EntityType == DTO.Enums.EntityType.DataSource);
            var listUser = controller.List().Where(x => x.EntityType == DTO.Enums.EntityType.User);

            Assert.IsTrue(listAll.Count() > 0);
            Assert.IsTrue(listOrg.Count() > 0);
            Assert.IsTrue(listDS.Count() > 0);
            //TODO: This is puposely set to false until User Domain Data is added
            Assert.IsFalse(listUser.Count() > 0);

        }
    }
}
