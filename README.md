# Reklamacka

Mobilní aplikace v [Xamarin](https://dotnet.microsoft.com/apps/xamarin) C# pro správu, uchování a editaci koupených produktů a jejich účtenek. 
Aplikace ukládá data o datumu nákupu, expiraci záruční lhůty, údajů o obchodu a dalších nezbytných či užitečných detailů. 
Databáze těchto objektů podporuje filtrování, řazení, vyhledávání a mnohé způsoby zobrazení položek.

Aplikace byla vytvořena jako výstupní práce týmového projektu v rámci předmětu ITU na FIT VUT 2021/2022

## Instalace
### Instalace pomocí zdrojových souborů
- Stažení zdrojových souborů z repozitáře.
- Rozbalit Solution soubor Reklamacka.sln v programu Microsoft Visual Studio.
- Pravým kliknutím v Solution Explorer na na Reklamacka.Android spustit volbu Archive.
- Po dokončení kliknout na Distribute, následně Ad Hoc, vyplnit formulář certifikátu, Save as a ve složce zvolené k vygenerování souboru je výsledný .apk soubor.
- Přemístit spustitelný .apk soubor na mobilní zařízení a spustit. 
- Aplikace se nainstaluje.

Podrobnější návod generování najdete na stránkách [první část](https://docs.microsoft.com/cs-cz/xamarin/android/deploy-test/release-prep/?tabs=windows#protect_app) a [druhá část](https://docs.microsoft.com/cs-cz/xamarin/android/deploy-test/signing/?tabs=windows).


### Možnost přímé instalace z vytvořeného .apk
- V oficiálním [repozitáři](https://github.com/Darbix/Reklamacka) k projektu je uložen .apk instalační soubor na systém Android, který je vygenerovaný předchozím způsobem. 
- Soubor stáhnout a přesunout na mobilní zařízení.
- Po jeho spuštění na zmíněném systému dojde přímo k instalaci.

## Knihovny
Aplikace využívá následující externí knihovny a balíčky:
- [Xamarin.Community.Toolkit](https://github.com/xamarin/XamarinCommunityToolkit)
- [Plugin.LocalNotification](https://github.com/thudugala/Plugin.LocalNotification)
- [Xamarin.Essentials](https://github.com/xamarin/Essentials)
- [SQLite-net](https://github.com/praeclarum/sqlite-net)

## Tvůrci
Hung Do (xdohun00)   
David Kedra (xkedra00)

## Licence
[MIT](https://choosealicense.com/licenses/mit/)
