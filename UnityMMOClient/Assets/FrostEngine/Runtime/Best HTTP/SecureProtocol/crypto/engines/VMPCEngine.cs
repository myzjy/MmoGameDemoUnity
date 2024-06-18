#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    public class VmpcEngine
        : IStreamCipher
    {
        /*
        * variables to hold the state of the VMPC engine during encryption and
        * decryption
        */
        protected byte N = 0;
        protected byte[] P = null;
        protected byte S = 0;

        protected byte[] WorkingIv;
        protected byte[] WorkingKey;

        public virtual string AlgorithmName
        {
            get { return "VMPC"; }
        }

        /**
        * initialise a VMPC cipher.
        * 
        * @param forEncryption
        *    whether or not we are for encryption.
        * @param params
        *    the parameters required to set up the cipher.
        * @exception ArgumentException
        *    if the params argument is inappropriate.
        */
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(parameters is ParametersWithIV))
                throw new ArgumentException("VMPC Init parameters must include an IV");

            ParametersWithIV ivParams = (ParametersWithIV)parameters;

            if (!(ivParams.Parameters is KeyParameter))
                throw new ArgumentException("VMPC Init parameters must include a key");

            KeyParameter key = (KeyParameter)ivParams.Parameters;

            this.WorkingIv = ivParams.GetIV();

            if (WorkingIv == null || WorkingIv.Length < 1 || WorkingIv.Length > 768)
                throw new ArgumentException("VMPC requires 1 to 768 bytes of IV");

            this.WorkingKey = key.GetKey();

            InitKey(this.WorkingKey, this.WorkingIv);
        }

        public virtual void ProcessBytes(
            byte[] input,
            int inOff,
            int len,
            byte[] output,
            int outOff)
        {
            Check.DataLength(input, inOff, len, "input buffer too short");
            Check.OutputLength(output, outOff, len, "output buffer too short");

            for (int i = 0; i < len; i++)
            {
                S = P[(S + P[N & 0xff]) & 0xff];
                byte z = P[(P[(P[S & 0xff]) & 0xff] + 1) & 0xff];
                // encryption
                byte temp = P[N & 0xff];
                P[N & 0xff] = P[S & 0xff];
                P[S & 0xff] = temp;
                N = (byte)((N + 1) & 0xff);

                // xor
                output[i + outOff] = (byte)(input[i + inOff] ^ z);
            }
        }

        public virtual void Reset()
        {
            InitKey(this.WorkingKey, this.WorkingIv);
        }

        public virtual byte ReturnByte(
            byte input)
        {
            S = P[(S + P[N & 0xff]) & 0xff];
            byte z = P[(P[(P[S & 0xff]) & 0xff] + 1) & 0xff];
            // encryption
            byte temp = P[N & 0xff];
            P[N & 0xff] = P[S & 0xff];
            P[S & 0xff] = temp;
            N = (byte)((N + 1) & 0xff);

            // xor
            return (byte)(input ^ z);
        }

        protected virtual void InitKey(
            byte[] keyBytes,
            byte[] ivBytes)
        {
            S = 0;
            P = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                P[i] = (byte)i;
            }

            for (int m = 0; m < 768; m++)
            {
                S = P[(S + P[m & 0xff] + keyBytes[m % keyBytes.Length]) & 0xff];
                byte temp = P[m & 0xff];
                P[m & 0xff] = P[S & 0xff];
                P[S & 0xff] = temp;
            }

            for (int m = 0; m < 768; m++)
            {
                S = P[(S + P[m & 0xff] + ivBytes[m % ivBytes.Length]) & 0xff];
                byte temp = P[m & 0xff];
                P[m & 0xff] = P[S & 0xff];
                P[S & 0xff] = temp;
            }

            N = 0;
        }
    }
}
#pragma warning restore
#endif