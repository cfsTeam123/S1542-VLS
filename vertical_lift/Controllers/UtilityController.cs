using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;


using System.Diagnostics;

using System.IO.Ports;
using System.Threading;
using Microsoft.Ajax.Utilities;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace vertical_lift.Controllers
{
    public class UtilityController : Controller
    {
        // GET: Utility
        public ActionResult Index()
        {
            return View();
        }



        //assigning finger if not assigned
        public string ExecuteCommand(string finger_name, string fid)
        {
            S1542Entities conn = new S1542Entities();

                //int counter = 0;
                var finger = (from FI in conn.finger_id
                          where FI.status != "ASSIGNED"
                          select new
                          {
                              MTransNo = FI.MTransNo,
                              finger = FI.finger
                          }).GroupBy(x => x.MTransNo).Select(v => v.FirstOrDefault()).OrderBy(x => x.MTransNo).FirstOrDefault();
            if (finger == null)
            {
                return "Maximum limit is reached to register in biometric module";
            }
            string output = "abc,cvcv";

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.Arguments = @"/c E:\\pdf2xml";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();

            //p.StandardInput.WriteLine("D:");

            p.StandardInput.WriteLine("python C:\\FINGERREGISTER\\scripts\\FingerEnroll.py " + finger_name + " " + finger.finger + " biometric_id" + fid + " 2");

            //p.StandardInput.WriteLine("cd \\scripts\\");
            // p.Stand"ardInput.WriteLine("python PythonSampleSystemDiagnostic.py 5 10");
            string d = "python C:\\FINGERREGISTER\\scripts\\FingerEnroll.py " + finger_name + " " + finger.finger + " biometric_id" + fid + " 2";
            int a = 0;

            while (true)
            {
                a++;
                try
                {
                    output = p.StandardOutput.ReadLine();
                    //if (a == 6)
                    //{
                    //    return "6";
                    //}

                    if (output.Contains("b'"))
                    {

                        string[] fingerdata = output.Split('\'');
                        string fing_id = fingerdata[0].Replace(" ", "_");
                        string[] fing_id1 = fing_id.Split('_');
                        if (fid == "1")  //fingerdata[2].Trim()
                        {

                            string i = (finger_name);
                           /* var emp_master = conn.UserMasters.FirstOrDefault(v => v.UserId == userId);
                            int i = Convert.ToInt32(finger_name);*/
                            var emp_master = conn.UserMasters.Where(v => v.UserId == i).FirstOrDefault();
                            if (emp_master.BioId1 != null)
                            {
                                int bi1 = Convert.ToInt32(emp_master.BioId1);
                                p.StandardInput.WriteLine("python C:\\FINGERREGISTER\\scripts\\FingerDelete.py " + emp_master.BioId1);
                                var f1 = conn.finger_id.Where(v => v.finger == bi1).FirstOrDefault();
                                f1.status = "AVAILABLE";
                                conn.Entry(f1).State = System.Data.Entity.EntityState.Modified;
                            }
                            emp_master.BioId1 = fing_id1[1].Trim();  //.Split('\'')[1];
                            emp_master.BioData1 = fingerdata[1];//.Split('\'')[1];
                            conn.Entry(emp_master).State = System.Data.Entity.EntityState.Modified;
                            int j = Convert.ToInt32(finger.finger);
                            var finger_tb = conn.finger_id.Where(v => v.finger == j).FirstOrDefault();
                            finger_tb.status = "ASSIGNED";
                            conn.Entry(finger_tb).State = System.Data.Entity.EntityState.Modified;
                            conn.SaveChanges();
                        }
                        else
                        {
                            string i = (finger_name);
                            var emp_master = conn.UserMasters.Where(v => v.UserId == i).FirstOrDefault();
                            if (emp_master.BioId2 != null)
                            {
                                int bi2 = Convert.ToInt32(emp_master.BioId2);
                                p.StandardInput.WriteLine("python C:\\FINGERREGISTER\\scripts\\FingerDelete.py " + emp_master.BioId2);
                                var f2 = conn.finger_id.Where(v => v.finger == bi2).FirstOrDefault();
                                f2.status = "AVAILABLE";
                                conn.Entry(f2).State = System.Data.Entity.EntityState.Modified;
                            }
                            emp_master.BioId2 = fing_id1[1].Trim();//.Split('\'')[1];
                            emp_master.BioData2 = fingerdata[1];//.Split('\'')[1];
                            conn.Entry(emp_master).State = System.Data.Entity.EntityState.Modified;
                            var finger_tb = conn.finger_id.Where(v => v.finger == finger.finger).FirstOrDefault();
                            finger_tb.status = "ASSIGNED";
                            conn.Entry(finger_tb).State = System.Data.Entity.EntityState.Modified;
                            conn.SaveChanges();
                        }
                    }
                    else if (output.Contains("Success1"))
                    {
                        break;
                    }
                    else if (output.Contains("Success2"))
                    {
                        return output;
                    }
                    else if (output.Contains("Verification Error"))
                    {
                        return output;
                    }
                    else if (output.Contains("Conversion Error"))
                    {
                        return "Please place finger properly and enroll again";
                    }
                    else if (output.Contains("Template Error1"))
                    {
                        return "Please place finger properly and enroll again";
                    }
                    else if (output.Contains("Template Error2"))
                    {
                        return "Please place finger properly and enroll again";
                    }
                    else if (output.Contains("Store Error"))
                    {
                        return "No space in Bio-metric Module,Please try again";
                    }
                    else if (output.Contains("Traceback"))
                    {
                        return "Error in communication with Bio-metric Module,Please try again";
                    }
                    else if (output.Contains("exited with code"))
                    {
                        return "Error in communication with Bio-metric Module,Please try again";
                    }
                    else if (output.Contains("Finger not Placed,Try Again"))
                    {
                        return "Finget Not Placed";
                    }
                    else if (output.Contains("could not open port"))
                    {
                        return "Could Not Open Port/Port Busy";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(output);
                    }

                }
                catch (Exception e)
                {
                    return e.ToString();
                }

            }
            return output;
        }

    }
}