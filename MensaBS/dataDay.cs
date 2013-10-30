using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MensaBS
{
    class dataDay
    {
        public List<dataMeal> lunch { get; set; }
        public List<dataMeal> dinner { get; set; }

        public dataDay()
        {
            lunch = new List<dataMeal>();
            dinner = new List<dataMeal>();
        }

        public void parse(XDocument doc)
        {
            lunch.Clear();
            dinner.Clear();


            XElement mittag = doc.Element("data").Element("tag").Element("mittag");
            lunch = (from meal in mittag.Descendants("essen")
                     select new dataMeal
        {
            name = meal.Element("name").Value,
            poison = meal.Element("remark").Value,

            priceStudent = (from price in meal.Elements("preis")
                            where price.Attribute("typ").Value == "stud"
                            select price.Value).Single(),

            priceEmployee = (from price in meal.Elements("preis")
                             where price.Attribute("typ").Value == "bed"
                             select price.Value).Single(),

            priceGuest = (from price in meal.Elements("preis")
                          where price.Attribute("typ").Value == "gast"
                          select price.Value).Single(),


        }).ToList<dataMeal>();


            XElement abend = doc.Element("data").Element("tag").Element("abend");
            dinner = (from meal in abend.Descendants("essen")
                      select new dataMeal
                      {
                          name = meal.Element("name").Value,
                          poison = meal.Element("remark").Value,

                          priceStudent = (from price in meal.Elements("preis")
                                          where price.Attribute("typ").Value == "stud"
                                          select price.Value).Single(),

                          priceEmployee = (from price in meal.Elements("preis")
                                           where price.Attribute("typ").Value == "bed"
                                           select price.Value).Single(),

                          priceGuest = (from price in meal.Elements("preis")
                                        where price.Attribute("typ").Value == "gast"
                                        select price.Value).Single(),


                      }).ToList<dataMeal>();

        }
    }
}
