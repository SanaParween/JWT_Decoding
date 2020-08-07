using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace JWT_Decoding.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public string TokenEntered { get; set; }
        //public string enteredToken => (string)TempData[ContentOf(TextArea1)];
        public string TextArea1 => (string)TempData[nameof(TextArea1)];

        public void OnGet()
        {
        }

        public IActionResult OnPost([FromForm] string TextArea1)
        {
            var jwt = TextArea1;
            //var decrypt = decrypted(jwt);
            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims;
            var header = token.Header;

            ViewData["claims"] = claims;
            ViewData["header"] = header;
            List<string> headerKey = new List<string>();
            List<string> headerValue = new List<string>();
            foreach (var head in header)
            {
                headerKey.Add(head.Key);
                headerValue.Add((string)head.Value);
            };
            ViewData["headerKey"] = headerKey;
            ViewData["headerValue"] = headerValue;

            return Page() ;
        }

        public string decrypted(string encryptPass)
        {


            byte[] data = Convert.FromBase64String(encryptPass);
            string decodedString = Encoding.UTF8.GetString(data);

            string decryptedPass = string.Empty;
            UTF8Encoding encodePass = new UTF8Encoding();
            Decoder Decode = encodePass.GetDecoder();
            byte[] toDecodeByte = Convert.FromBase64String(encryptPass);
            int charCount = Decode.GetCharCount(toDecodeByte, 0, toDecodeByte.Length);
            char[] decodedChar = new char[charCount];
            Decode.GetChars(toDecodeByte, 0, toDecodeByte.Length, decodedChar, 0);
            decryptedPass = new String(decodedChar);
            return decryptedPass;

        }
        private static byte[] ConvertFromBase64String(string input)
        {
            string working = input.Replace('-', '+').Replace('_', '/');
            while (working.Length % 3 != 0)
            {
                working += '=';
            }
            try
            {
                return Convert.FromBase64String(working);
            }
            catch (Exception)
            {
                // .Net core 2.1 bug
                // https://github.com/dotnet/corefx/issues/30793
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/'));
                }
                catch (Exception) { }
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "=");
                }
                catch (Exception) { }
                try
                {
                    return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "==");
                }
                catch (Exception) { }

                return null;
            }
        }
    }
}
