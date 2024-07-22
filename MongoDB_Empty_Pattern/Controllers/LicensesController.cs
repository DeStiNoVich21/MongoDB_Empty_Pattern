using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB_Empty_Pattern.Models;
using MongoDB_Empty_Pattern.MongoRepository.GenericRepository;
using MongoDB_Empty_Pattern.Services;
using System;
using System.Threading.Tasks;

namespace MongoDB_Empty_Pattern.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class LicensesController : Controller
    {
        private readonly IMongoRepository<Licenses> _licensesCollection;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public LicensesController(IMongoRepository<Licenses> dbRepository, IConfiguration configuration, IUserService userRepository)
        {
            _licensesCollection = dbRepository;
            _configuration = configuration;
            _userService = userRepository;
        }
        /// <summary>
        /// Привязка MAC-адреса к лицензии и проверка соответствия
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        [HttpPost("bindMacAddress")]
        public async Task<ActionResult> BindMacAddress([BindRequired] string licenseKey, [BindRequired] string macAddress)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == licenseKey);

                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }

                // Проверка на статус лицензии
                if (license.status.ToLower() == "false")
                {
                    return StatusCode(403, "Лицензия не активирована");
                }

                // Проверка на истечение срока действия лицензии
                if (license.expire_date < DateTime.UtcNow)
                {
                    return StatusCode(403,"Лицензия просрочена");
                }

                if (string.IsNullOrEmpty(license.mac_address))
                {
                    // Привязка MAC-адреса
                    license.mac_address = macAddress; 
                    _licensesCollection.ReplaceOne(license);
                    return Ok("MAC-адрес успешно привязан к лицензии");
                }
                else if (license.mac_address == macAddress)
                {
                    // MAC-адрес совпадает
                    return Ok("MAC-адрес совпадает с существующим");
                }
                else
                {
                    // MAC-адрес не совпадает
                    return Conflict("MAC-адрес не совпадает с существующим");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        /// Создание лицензии
        /// </summary>
        /// <param name="org"></param>
        /// <param name="expire_date"></param>
        /// <param name="BIN"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("createLicense")]
        public async Task<ActionResult> CreateLicense([BindRequired] string org, [BindRequired] DateTime expire_date, [BindRequired] string Bin)
        {
            try
            {
                var license = new Licenses
                {
                    mac_address = string.Empty, // MAC address будет пустым
                    licences_date = expire_date.ToString("yyyy-MM-dd"),
                    licences_key = Guid.NewGuid().ToString(), // Генерация случайного ключа лицензии
                    Org = org,
                    BIN = Bin,
                    status = "False", // Статус по умолчанию false
                    expire_date = expire_date
                };

                _licensesCollection.InsertOne(license);
                return Ok(new { status_code = "200", body = license });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Активация лицензии
        /// </summary>
        /// <param name="License_Code"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("activateLicense/{License_Code}")]
        public async Task<ActionResult> ActivateLicense(string License_Code)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == License_Code);

                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }

                license.status = "True";
                _licensesCollection.ReplaceOne(license);
                return Ok(license);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        /// Деактивация лицензии
        /// </summary>
        /// <param name="License_Code"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("deactivateLicense/{License_Code}")]
        public async Task<ActionResult> DeactivateLicense(string License_Code)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == License_Code);
                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }
                license.status = "False";
                _licensesCollection.ReplaceOne(license);
                return Ok(license);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Удаление лицензии
        /// </summary>
        /// <param name="License_Code"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("deleteLicense/{License_Code}")]
        public async Task<ActionResult> DeleteLicense(string License_Code)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == License_Code);
                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }
                _licensesCollection.DeleteOne(l => l.licences_key == License_Code);
                return Ok(new { status_code = "200", message = "Лицензия удалена" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Продление лицензии
        /// </summary>
        /// <param name="License_Code"></param>
        /// <param name="newExpireDate"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("extendLicense/{License_Code}")]
        public async Task<ActionResult> ExtendLicense(string License_Code, [BindRequired] DateTime newExpireDate)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == License_Code);
                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }
                license.expire_date = newExpireDate;
                license.licences_date = newExpireDate.ToString("yyyy-MM-dd");
                _licensesCollection.ReplaceOne(license);
                return Ok(license);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение одной лицензии
        /// </summary>
        /// <param name="License_Code"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getLicense/{License_Code}")]
        public async Task<ActionResult> GetLicense(string License_Code)
        {
            try
            {
                var license = await _licensesCollection.FindOne(l => l.licences_key == License_Code);
                if (license == null)
                {
                    return NotFound("Лицензия не найдена");
                }
                return Ok(license);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение всех лицензий
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getAllLicenses")]
        public async Task<ActionResult> GetAllLicenses()
        {
            try
            {
                var licenses = await _licensesCollection.GetAllAsync();
                return Ok(licenses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
