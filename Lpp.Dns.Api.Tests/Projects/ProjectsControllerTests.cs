using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Lpp.Dns.Api.Projects;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using System.Threading.Tasks;
using Lpp.Utilities;

namespace Lpp.Dns.Api.Tests.Projects
{
    [TestClass]
    public class ProjectsControllerTests : DataControllerTest<Api.Projects.ProjectsController, ProjectDTO, Project>
    {
        public ProjectsControllerTests()
            : base(new ProjectDTO
            {
                Acronym = "Test",
                Active = true,
                Deleted = false,
                Description = "Test Project",
                GroupID = null,
                ID = null,
                Name = "Test Project",
                StartDate = DateTime.Today
            }) { }

        [TestMethod]
        public async Task ProjectsCopy()
        {
            var projectID = await controller.Copy(new Guid("06C20001-1C79-4260-915E-A22201477C58"));
        }

        [TestMethod]
        public async Task ProjectsGet() { await GetTest(); }

        [TestMethod]
        public void ProjectsList() { ListTest(); }

        [TestMethod]
        public async Task ProjectsInsert() { await InsertTest(); }

        [TestMethod]
        public async Task ProjectsInsertOrDelete() { await InsertOrUpdateTest(); }

        [TestMethod]
        public async Task ProjectsUpdate() { await UpdateTest(); }

        [TestMethod]
        public async Task ProjectsDelete() { await DeleteTest(); }

        [TestMethod]
        public void GetAvailableQERoutes()
        {

            using(var db = new DataContext())
            {
                db.Database.Log = Console.WriteLine;

                var q = (from pdm in db.ProjectDataMarts
                        join p in db.Projects on pdm.ProjectID equals p.ID
                        join dm in db.DataMarts on pdm.DataMartID equals dm.ID
                        join prt in db.ProjectRequestTypes on p.ID equals prt.ProjectID
                        let dmAcl = db.DataMartRequestTypeAcls.Where(a => a.DataMartID == dm.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                        let prtACL = db.ProjectRequestTypeAcls.Where(a => a.ProjectID == p.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                        let pdmrtACL = db.ProjectDataMartRequestTypeAcls.Where(a => a.ProjectID == p.ID && a.DataMartID == dm.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                        where
                        p.Active && !p.Deleted && (!p.EndDate.HasValue || p.EndDate.Value > DateTime.UtcNow) && (p.StartDate <= DateTime.UtcNow)
                        && dm.AdapterID.HasValue && dm.Deleted == false
                        && (
                            (dmAcl.Any() || prtACL.Any() || pdmrtACL.Any()) &&
                            (dmAcl.All(a => a > 0) && prtACL.All(a => a > 0) && pdmrtACL.All(a => a > 0))
                        )
                        orderby p.Name, prt.RequestType.Name, dm.Name
                        select new
                        {
                            ProjectID = p.ID,
                            Project = p.Name,
                            pdm.DataMartID,
                            DataMart = dm.Name,
                            prt.RequestTypeID,
                            RequestType = prt.RequestType.Name
                        }).ToArray();                

                foreach(var rt in q)
                {
                    Console.WriteLine(string.Format("Project: {0}\t RequestType: {1}\t DataMart: {2}", rt.Project, rt.DataMart, rt.RequestType));
                }

                var x = q.GroupBy(k => new { k.ProjectID, k.Project }).Select(k => new
                {
                    Project = k.Key.Project,
                    ProjectID = k.Key.ProjectID,
                    DataMarts = k.GroupBy(v => new { v.DataMartID, v.DataMart })
                                .Select(v => new
                                {
                                    DataMart = v.Key.DataMart,
                                    DataMartID = v.Key.DataMartID,
                                    RequestTypes = v.Select(u => new { u.RequestType, u.RequestTypeID }).OrderBy(rt => rt.RequestType)
                                }).OrderBy(dm => dm.DataMart)
                }).OrderBy(p => p.Project);


                var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                Console.WriteLine(serialized);

            }

        }

    }
}
