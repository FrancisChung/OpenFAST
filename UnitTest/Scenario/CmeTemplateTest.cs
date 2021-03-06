/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System.IO;
using NUnit.Framework;
using OpenFAST.Template.Loader;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Scenario
{
    [TestFixture]
    public class CmeTemplateTest : OpenFastTestCase
    {
        [Test]
        public void TestDeltas()
        {
            var templateLoader = new XmlMessageTemplateLoader { LoadTemplateIdFromAuxId = true };

            using (var stream = File.OpenRead("CME/templates.xml"))
            {
                templateLoader.Load(stream);
            }

            using (var stream = File.OpenRead("CME/messages.fast"))
            {
                var mis = new MessageInputStream(stream, templateLoader.TemplateRegistry);
                Message md = mis.ReadMessage();
                Assert.AreEqual(-5025.0, md.GetSequence("MDEntries")[0].GetDouble("NetChgPrevDay"), .1);
            }
        }
    }
}