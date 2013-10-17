using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;



/*
  <essen nummer="8">
				<name>1/2 BrathÃ¤hnchen </name>
				<remark>G</remark>
				<preis typ="stud">1,90</preis>
				<preis typ="bed">3,40</preis>
				<preis typ="gast">4,00</preis>
			</essen>
 */

namespace MensaBS
{
    public class Meal
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("remark")]
        public string poison { get; set; }


        [XmlElement("preis")]
        public ObservableCollection<price> prices { get; set; }

        public class price
        {
            [XmlAttribute("typ")]
            public string type { get; set; }

            [XmlText]
            public string amount;

        }


    }
}
