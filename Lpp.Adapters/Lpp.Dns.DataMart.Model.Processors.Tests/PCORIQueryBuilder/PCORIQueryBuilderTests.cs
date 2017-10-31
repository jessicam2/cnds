using Lpp.Dns.DataMart.Model.PCORIQueryBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Entity;
using System.Collections;

namespace Lpp.Dns.DataMart.Model.Processors.Tests.PCORIQueryBuilder
{
    [TestClass]
    public class PCORIQueryBuilderTests
    {
        [TestMethod]
        public void PCORI_QueryPatients()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var results = db.Patients.Take(10).ToArray();
                foreach (var r in results)
                {
                    Console.WriteLine(r.ID);
                }

                DataTable dt = new DataTable();
                var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.Hispanic, r.ID, r.Race }, LoadOption.OverwriteChanges));

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                var i = ds.Tables.Count;
            }


        }

        [TestMethod]
        public void PCORI_QueryDiagnoses()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var results = db.Diagnoses.Take(10).ToArray();
                foreach (var r in results)
                {
                    Console.WriteLine(r.PatientID);
                }

                DataTable dt = new DataTable();
                var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.Code, r.PatientID, r.CodeType }, LoadOption.OverwriteChanges));

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                var i = ds.Tables.Count;
            }


        }

        [TestMethod]
        public void PCORI_QueryEncounters()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var results = db.Encounters.Take(10).ToArray();
                foreach (var r in results)
                {
                    Console.WriteLine(r.PatientID);
                }

                DataTable dt = new DataTable();
                var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.ID, r.PatientID, r.EncounterType }, LoadOption.OverwriteChanges));

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                var i = ds.Tables.Count;
            }


        }

        [TestMethod]
        public void PCORI_QueryEnrollments()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var results = db.Enrollments.Take(10).ToArray();
                foreach (var r in results)
                {
                    Console.WriteLine(r.PatientID);
                }

                DataTable dt = new DataTable();
                var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.EncrollmentBasis, r.PatientID, r.StartedOn }, LoadOption.OverwriteChanges));

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                var i = ds.Tables.Count;
            }


        }

        [TestMethod]
        public void PCORI_QueryProcedures()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var results = db.Procedures.Take(10).ToArray();
                foreach (var r in results)
                {
                    Console.WriteLine(r.PatientID);
                }

                DataTable dt = new DataTable();
                var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.Code, r.PatientID, r.CodeType }, LoadOption.OverwriteChanges));

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                var i = ds.Tables.Count;
            }


        }

        [TestMethod]
        public void PCORI_QueryVitals()
        {
            //string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            //using (var db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            //{
            //    var results = db.Vitals.Take(10).ToArray();
            //    foreach (var r in results)
            //    {
            //        Console.WriteLine(r.PatientID);
            //    }

            //    DataTable dt = new DataTable();
            //    var asDataTable = results.Select(r => dt.LoadDataRow(new object[] { r.Height, r.PatientID, r.Weight }, LoadOption.OverwriteChanges));

            //    DataSet ds = new DataSet();
            //    ds.Tables.Add(dt);

            //    var i = ds.Tables.Count;
            //}


        }

        [TestMethod]
        public void PCORI_ConfirmMappings()
        {
            string connectionString = "Server=(local);User Id=sa;Password=dyknalna;Database=PCORNETV2;";
            using (DataContext db = Lpp.Dns.DataMart.Model.PCORIQueryBuilder.DataContext.Create(connectionString))
            {
                var demographic = db.Patients.First();
                var diagnosis = db.Diagnoses.First();
                var encounter = db.Encounters.First();
                var enrollment = db.Enrollments.First();
                var procedure = db.Procedures.First();
                //var vital = db.Vitals.First();
            }
        }


    }
}
