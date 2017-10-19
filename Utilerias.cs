using Chilkat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace SolucionesDC_Factura33
{
    public class Utilerias
    {
        private string Obtener_NoCertificado(byte[] pCertificado)
        {
            Cert loCert = new Cert();
            Rsa loRsa = new Rsa();

            bool success = true;

            // Desbloquea componente
            success = loRsa.UnlockComponent("RSAT34MB34N_7F1CD986683M");
            if (success != true)
            {
                throw new ArgumentException("No se pudo debloquear componente Chilkat RSA");
            }

            success = loCert.LoadFromBinary(pCertificado);

            if (success != true)
            {
                throw new ArgumentException("No se pudo cargar el archivo certificado");
            }

            string N = loCert.SerialNumber;

            string NoCertificado = N.Substring(02 - 1, 1) +
                            N.Substring(04 - 1, 1) +
                            N.Substring(06 - 1, 1) +
                            N.Substring(08 - 1, 1) +
                            N.Substring(10 - 1, 1) +
                            N.Substring(12 - 1, 1) +
                            N.Substring(14 - 1, 1) +
                            N.Substring(16 - 1, 1) +
                            N.Substring(18 - 1, 1) +
                            N.Substring(20 - 1, 1) +
                            N.Substring(22 - 1, 1) +
                            N.Substring(24 - 1, 1) +
                            N.Substring(26 - 1, 1) +
                            N.Substring(28 - 1, 1) +
                            N.Substring(30 - 1, 1) +
                            N.Substring(32 - 1, 1) +
                            N.Substring(34 - 1, 1) +
                            N.Substring(36 - 1, 1) +
                            N.Substring(38 - 1, 1) +
                            N.Substring(40 - 1, 1);

            return NoCertificado;
        }

        private string Obtener_Certificado(byte[] pCertificado)
        {
            Cert loCert = new Cert();
            Rsa loRsa = new Rsa();

            bool success = true;

            // Desbloquea componente
            success = loRsa.UnlockComponent("RSAT34MB34N_7F1CD986683M");
            if (success != true)
            {
                throw new ArgumentException("No se pudo debloquear componente Chilkat RSA");
            }

            success = loCert.LoadFromBinary(pCertificado);

            if (success != true)
            {
                throw new ArgumentException("No se pudo cargar el archivo certificado");
            }

            string strCertificado = loCert.GetEncoded();
            int LongitudCer = strCertificado.Length;

            strCertificado = strCertificado.Substring(1 - 1, LongitudCer - 2);

            return strCertificado;
        }

        public string Obtener_CadenaOriginal(Comprobante pComprobante, string pDirCadenaOriginal_xslt)
        {
            string preXML = pComprobante.Serialize();

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(preXML));
            XPathDocument myXPathDoc = new XPathDocument(ms);

            //Cargando el XSLT
            string DirEstructuraCedenaOriginal = pDirCadenaOriginal_xslt;
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(DirEstructuraCedenaOriginal);

            StringWriter str = new StringWriter();
            XmlTextWriter myWriter = new XmlTextWriter(str);

            //Aplicando transformacion
            myXslTrans.Transform(myXPathDoc, null, myWriter);

            //Resultado
            string result = str.ToString();

            //Fin del programa.
            return result;
        }

        public string Obtener_Sello(byte[] pKey, string pContraseniaCertificado, string pCadenaOriginal)
        {
            string strSello = string.Empty;
            string strLlavePwd = pContraseniaCertificado;
            string strCadenaOriginal = pCadenaOriginal;
            System.Security.SecureString passwordSeguro = new System.Security.SecureString();
            passwordSeguro.Clear();
            foreach (char c in strLlavePwd.ToCharArray())
                passwordSeguro.AppendChar(c);
            byte[] llavePrivadaBytes = pKey;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, passwordSeguro);
            System.Security.Cryptography.SHA256CryptoServiceProvider hasher = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] bytesFirmados = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(strCadenaOriginal), hasher);
            strSello = Convert.ToBase64String(bytesFirmados);

            return strSello;
        }

        public string Obtener_XML(Comprobante pComprobante)
        {
            string XML = pComprobante.Serialize();
            return XML;
        }

        public XElement Obtener_TimbreFiscalDigital(Comprobante pComprobante)
        {
            string XML = pComprobante.Serialize();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);

            MemoryStream xmlStream = new MemoryStream();
            doc.Save(xmlStream);

            xmlStream.Flush();
            xmlStream.Position = 0;

            var xdoc = XDocument.Load(xmlStream);

            var timbreFiscal = (from r in xdoc.Descendants()
                                where r.Name.LocalName == "TimbreFiscalDigital"
                                select r).First();

            return timbreFiscal;
        }

        public string Obtener_UUID(Comprobante pComprobante)
        {
            string XML = pComprobante.Serialize();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);

            MemoryStream xmlStream = new MemoryStream();
            doc.Save(xmlStream);

            xmlStream.Flush();
            xmlStream.Position = 0;

            var xdoc = XDocument.Load(xmlStream);

            var timbreFiscal = (from r in xdoc.Descendants()
                                where r.Name.LocalName == "TimbreFiscalDigital"
                                select r).First();

            return timbreFiscal.Attribute("UUID").Value;
        }
    }
}
