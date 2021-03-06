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
using NUnit.Framework;
using OpenFAST.Codec;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests.Scenario
{
    [TestFixture]
    public class ErrorCasesTest : OpenFastTestCase
    {
        [Test]
        public void TestDictionaryNotInherited()
        {
            const string templateDef = "<template name='OptDeltaDec' id='58' dictionary='template'>" +
                                       "    <string name='desc'/>" +
                                       "    <decimal id='1' presence='optional' name='Line1'>" +
                                       "         <exponent><copy/></exponent>" +
                                       "         <mantissa><copy/></mantissa>" +
                                       "    </decimal>" +
                                       "    <decimal id='1' presence='optional' name='Line2'>" +
                                       "         <exponent><copy/></exponent>" +
                                       "         <mantissa><copy/></mantissa>" +
                                       "    </decimal>    " +
                                       "    <decimal id='1' presence='optional' name='Line3'>" +
                                       "         <exponent><copy/></exponent> " +
                                       "         <mantissa><copy/></mantissa>" +
                                       "    </decimal>" +
                                       "</template>";

            MessageTemplate template = Template(templateDef);

            var m = new Message(template);

            m.SetString("desc", "prev");
            m.SetDecimal("Line2", 9427.61);
            m.SetDecimal("Line3", 9427.6);

            byte[] bytes = Encoder(template).Encode(m);
            Message m2 = Decoder(template, bytes).ReadMessage();

            Assert.AreEqual(m, m2);
        }

        [Test]
        public void TestEncodeDecodeNestedSequence()
        {
            var nestedSequence = new Sequence("nested",
                                              new Field[]
                                                  {
                                                      new Scalar("string", FastType.Ascii, Operator.Copy,
                                                                 ScalarValue.Undefined, false)
                                                  }, true);
            var group = new Group("group", new Field[] {nestedSequence}, true);
            var t = new MessageTemplate("template", new Field[] {group});
            var message = new Message(t);

            FastEncoder encoder = Encoder(t);
            AssertEquals("11000000 10000001", encoder.Encode(message));

            FastDecoder decoder = Decoder("11000000 10000001", t);
            Assert.AreEqual(message, decoder.ReadMessage());
        }

        [Test]
        public void TestMantissaIsntPresentWhenExponentIsNull()
        {
            const string templateXml = "<template name='SampleTemplate'>" +
                                       "  <decimal name='bid' presence='optional'>" +
                                       "    <mantissa><copy/></mantissa>" +
                                       "    <exponent><copy value='-2'/></exponent>" +
                                       "  </decimal>" +
                                       "</template>";
            MessageTemplate template = Template(templateXml);
            FastEncoder encoder = Encoder(template);

            var message = new Message(template);
            message.SetDecimal(1, 0.63);
            AssertEquals("11010000 10000001 10111111", encoder.Encode(message));

            message = new Message(template);
            AssertEquals("10100000 10000000", encoder.Encode(message));
        }
    }
}