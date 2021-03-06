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
using System;
using System.IO;
using OpenFAST.Error;

namespace OpenFAST.Template.Types.Codec
{
    internal sealed class NullableSingleFieldDecimal : TypeCodec
    {
        public static ScalarValue DefaultValue
        {
            get { return new DecimalValue(0.0); }
        }

        public override bool IsNullable
        {
            get { return true; }
        }

        public override byte[] EncodeValue(ScalarValue v)
        {
            if (v == ScalarValue.Null)
            {
                return NullValueEncoding;
            }

            var buffer = new MemoryStream();
            var value = (DecimalValue) v;

            try
            {
                if (Math.Abs(value.Exponent) > 63)
                {
                    Global.ErrorHandler.OnError(null, RepError.LargeDecimal, "");
                }

                byte[] tmp = NullableInteger.Encode(new IntegerValue(value.Exponent));
                buffer.Write(tmp, 0, tmp.Length);

                tmp = Integer.Encode(new LongValue(value.Mantissa));
                buffer.Write(tmp, 0, tmp.Length);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }

            return buffer.ToArray();
        }

        public override ScalarValue Decode(Stream inStream)
        {
            ScalarValue exp = NullableInteger.Decode(inStream);

            if ((exp == null) || exp.IsNull)
            {
                return null;
            }

            int exponent = exp.ToInt();
            long mantissa = Integer.Decode(inStream).ToLong();
            var decimalValue = new DecimalValue(mantissa, exponent);

            return decimalValue;
        }

        public static ScalarValue FromString(string value)
        {
            return new DecimalValue(Double.Parse(value));
        }

        public override bool Equals(Object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}