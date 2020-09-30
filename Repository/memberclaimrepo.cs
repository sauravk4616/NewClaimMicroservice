using claimmicroservice.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace claimmicroservice.Repository
{
    public class memberclaimrepo : Imemberclaimrepo
    {
        Uri baseAddress = new Uri("http://20.193.137.23/api");   //Port No.= https://localhost:44367/api
        HttpClient client;
        public memberclaimrepo()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        //do we need to make the list static
        public static List<memberclaim> m = new List<memberclaim>()
        {
            new memberclaim()
            {
                memberid=1,
                claimid=19172,
                billedamount=1200,
                claimedamount=1000,
                claimstatus="PENDING",
                benefitid=1
            },
            new memberclaim()
            {
                memberid=2,
                claimid=18246,
                billedamount=1200,
                claimedamount=1000,
                claimstatus="PENDING",
                benefitid=1
            },
            new memberclaim()
            {
                memberid=3,
                claimid=18940,
                billedamount=1200,
                claimedamount=1000,
                claimstatus="PENDING",
                benefitid=1
            }
         };

        public void create(memberclaim ob)
        {
            Random rand = new Random();
            ob.claimid = rand.Next(15000, 18000);
            ob.claimstatus = "PENDING";
            m.Add(ob);
        }
        //it is not used anymore
        public List<memberclaim> fetchclaimsformember(int id)//here id is the member id
        {
            List<memberclaim> l = new List<memberclaim>();
            foreach (var item in m)//fetch all the claims for a particular memberid
            {
                if (item.memberid == id)
                {
                    l.Add(item);
                }
            }
            return l;
        }
        public List<memberclaim> give()
        {

            return m;
        }
        //it is not used anymore
        public memberclaim getclaim(int id)//it returns an object of class type
        {
            memberclaim y = new memberclaim();
            foreach (var item in m)//fetch a single claim for a particular claimid
            {
                if (item.claimid == id)
                {
                    y = item;//eivabe ki direct kora jai na ei vabe kora jai eta to just ekta assign
                    //y.claimstatus = item.status;
                    break;
                }
            }
            return y;
        }

        public memberclaim GetClaimStatus(int id, memberclaim obj)
        {
            string s1 = obj.claimstatus;
            List<int> ls = new List<int>();
            int p = 0;
            int op = 0;

            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/policy/1/2").Result;//[100,200,300,400]]
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ls = JsonConvert.DeserializeObject<List<int>>(data);
            }
            HttpResponseMessage response1 = client.GetAsync(client.BaseAddress + "/policy/" + id).Result;//used to fetch the policyid of that particular memberid
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                op = Convert.ToInt32(data);               //it is giving corrcet result #policyid
                p = JsonConvert.DeserializeObject<int>(data);//it is becoming 0 i don't know
            }
            int d = obj.benefitid;
            HttpResponseMessage response2 = client.GetAsync(client.BaseAddress + "/policy/" + op + "/" + id + "/" + d).Result;
            int o = 0;
            if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                o = Convert.ToInt32(data);               //it is giving corrcet result #policyid
                                                         // p = JsonConvert.DeserializeObject<int>(data);//it is becoming 0 i don't know
            }
            if (obj.claimedamount > obj.billedamount)//if the bill is very less
            {
                //  return "Rejected";
                obj.claimstatus = "REJECTED";
            }
            else if (obj.claimedamount > o)//it checks for all the benefit ids also for benefitid even when no benefit id is selected
            {
                obj.claimstatus = "REJECTED";
            }
            else
            {
                obj.claimstatus = "ACCEPTED";
            }
            HttpResponseMessage response3 = client.GetAsync(client.BaseAddress + "/policy/" + op + "/" + id + "/" + d + "/1").Result;
            int qo = 0;//it has topup
            if (response3.IsSuccessStatusCode)
            {
                string data = response3.Content.ReadAsStringAsync().Result;
                qo = Convert.ToInt32(data);
            }
            if (qo == 0)
                obj.claimstatus = "REJECTED";
            memberclaimrepo x = new memberclaimrepo();
            foreach (var item in memberclaimrepo.m)
            {
                if (item.claimid == obj.claimid)
                    item.claimstatus = obj.claimstatus;
            }
            return obj;

        }
    }
}
