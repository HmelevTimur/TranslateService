Для работы Yandex Api необходимо получить Api key и добавить его в секреты, сделать это можно так:  
1)dotnet user-secrets init       
2)dotnet user-secrets set "YandexTranslate:ApiKey" "ваш api key"  
После чего можем запускать YandexTranslateService, а далее Client
