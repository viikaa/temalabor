# React és ASP.NET Core alapú webalkalmazás fejlesztése

A konkrét feladat egy  **teendőket kezelő webalkalmazás**  backendjének és frontendjének elkészítése. A teendőket adatbázisban tároljuk és a webes felületen jelenítjük meg, a kiszolgáló pedig REST interfészen keresztül érhető el.

A teendők rendelkeznek címmel, leírással, határidővel és felhasználó által létrehozott státuszok valamelyikével. A határidő helyett a prioritást a teendők sorrendje határozza meg, tehát az előbbi adataik mellett még az egymáshoz képesti sorrendet is tároljuk és megjelenítjük.

## Frontend

### Teendők megjelenítése
Az alkalmazás megjelenítésért felelős része egy egyszerű, routing nélküli, egy darab felületből álló React alkalmazás. Ezen a felületen A teendők oszlopokba rendezve jelennek meg. 

Ezek az oszlopok a felhasználó által létrehozott státuszokat reprezentálják. Oszlopot (státuszt) lehet létrehozni, törölni (ekkor a hozzá tartozó teendők is törlődnek) és annak nevét módosítani azzal a kitétellel, hogy nem létezhet egyszerre több ugyanolyan nevű oszlop. 

A teendők listázásakor a felhasználó csak azok neveit látja. Lehetőség van törölni teendőt, illetve megjeleníteni annak részleteit egy modális ablakon, aminek segítségével egyes adatait módosítani is lehet a teendőknek. Státusz módosításakor az az újonnan beállított státuszt reprezentáló oszlopban jelenik meg.

### Kommunikáció a szerverrel
A kommunikáció a szerverrel *Fetch API*-val történik. A kommunikáció megvalósítására szolgáló függvények 2 fájlba vannak szerveze: `columnsApi.js`, `todosApi.js`. 
A megjelenítésért szolgáló komponenseken történő módosítások során hívódnak az ezekben a fájlokban definiált, *CRUD* műveleteket megvalósító függvények.
Egy harmadik fájlból (`baseUrl.js`) van exportálva a a szerver eléréséhez szükséges url cím, így ezt változás esetén kettő helyett elég egy helyen módosítani.
#### columnsApi.js
##### `fetchColumns`
Az összes adatbázisban lévő oszlopot kéri le, és azok tömbjével tér vissza.
##### `fetchSingleColumn`
A paraméterként kapott id-val megegyező id-jú oszlopot kéri le és azzal tér vissza
##### `addColumn`
Új oszlop hozzáadására szolgál. A paraméterként kapott oszlop objektumot elküldi a szervernek egy *POST* kérés törzsében *JSON* objektumként. Amennyiben a válasz státuszkódja 409 (*conflict*) volt, null-lal tér vissza, egyébként a válasz törzsével. Erre azért van szükség, mert egyedül az oszlopok nevének egyediségét nem tudja validálni a form, így ezt az esetet külön kezeljük.
##### `updateColumn`
Oszlop módosítására szolgál. A paraméterként kapott oszlop objektumot elküldi a szervernek egy *PUT* kérés törzsében *JSON* objektumként. A 409-es (*conflict*) státuszkódot itt is külön kezeljük, azzal az eltéréssel, hogy mivel a szerver *PUT* kérés kiszolgálása során mindenképp üres törzsű válasszal tér vissza, `false` értéket adunk vissza ebben az esetben, egyébként `true`-t.
##### `deleteColumn`
A paraméterként kapott id-val megegyező id-jú oszlopot törli az adatbázisból.
#### todosApi.js
##### `fetchTodos`
Az összes adatbázisban lévő teendőt kéri le, és azok tömbjével tér vissza.
##### `fetchSingleTodo`
A paraméterként kapott id-val megegyező id-jú teendőt kéri le és azzal tér vissza
##### `addTodo`
Új teendő hozzáadására szolgál. A paraméterként kapott teendő objektumot elküldi a szervernek egy *POST* kérés törzsében *JSON* objektumként. Amennyiben a válasz státuszkódja 409 (*conflict*) volt, null-lal tér vissza, egyébként a válasz törzsével. Erre azért van szükség, mert egyedül a teendők nevének (oszlopbeli) egyediségét nem tudja validálni a form, így ezt az esetet külön kezeljük.
##### `updateTodo`
Teendő módosítására szolgál. A paraméterként kapott teendő objektumot elküldi a szervernek egy *PUT* kérés törzsében *JSON* objektumként. A 409-es (*conflict*) státuszkódot itt is külön kezeljük, azzal az eltéréssel, hogy mivel a szerver *PUT* kérés kiszolgálása során mindenképp üres törzsű válasszal tér vissza, `false` értéket adunk vissza ebben az esetben, egyébként `true`-t.
##### `deleteTodo`
A paraméterként kapott id-val megegyező id-jú teendőt törli az adatbázisból.
#### baseUrl.js
A `baseUrl` konstans a szerver url címét tartalmazza.

### Alkalmazás kontextusának frissítése

#### DataContextProvider.js
Lehetővé teszi a szervertől kapott oszlopok és teendők elérését a gyerekkomponensek számára a React által biztosított *Context API* és `useReducer` *hook* segítségével. Az állapot kezélésre A `DataReducer.js` fájlban található reducer segítségével történik.
#### DataReducer.js
Három *actionType*-ot definiál, amikkel módosítható a `DataContextProvider` által a gyerekkomponensei számára elérhetővé tett, oszlopokat és teendőket tartalmazó globális állapot.
##### `UPDATE_DATA`
A teljes állapotot lecseréli a `DataContextProvider` által bisztosított `dispatch` függvényben paraméterül kapott `action` objektum `payload` mezőjére.
##### `UPDATE_COLUMNS`
Az oszlopok tömbjét lecseréli a `DataContextProvider` által bisztosított `dispatch` függvényben paraméterül kapott `action` objektum `payload.columns` mezőjére.
##### `UPDATE_TODOS`
Az oszlopok tömbjét lecseréli a `DataContextProvider` által bisztosított `dispatch` függvényben paraméterül kapott `action` objektum `payload.todos` mezőjére, miután azokat `priority` szerint rendezte. Erre azért van szükség kliensoldalon, hogy rendezés miatt ne kelljen a szerverhez fordulni módosítások során.

### Megjelenítő komponensek
#### index.js
Az alkalmazást megjelenítő szkript. Az alkalmazást vázát adó index.html fájljának *root* id-val ellátott div elemébe rendereli ki az `App` komponenst a `DataContextProvider` komponens gyerekeként.

#### App.js
Az alkalmazás "belépési pontjaként" szolgál. Ez a komponens minden további komponens őse. Renderelés előtt `useEffect` *hook* használatával lekéri a szervertől a meglévő oszlopokat és teendőket, és a `DataContextProvider` által biztosított `dispatch` függvény segítségével elmenti a globális állapotba.
Renderelés során megjeleníti a lekért oszlopokat (amennyiben vannak), és megjelenít egy `Placeholder` komponenst, ami új oszlop felvételére szolgál.
#### Column.js
Egy oszlop megjelenítését, illetve az oszlop szerkesztésére és törlésére szolgáló modális ablak renderelését és láthatóságának kezelését végzi.  `useEffect` *hook* segítségével renderelés előtt és a globális állapot változásakor a `DataContextProvider` által biztosított globális állapotból lekéri a hozzátartozó teendőket és eltárolja saját állapotban. Három saját állapotot tart nyilván a komponens:

|Állapot|Mit tárol|
|--|--|
|`thisColumn`|Tartalmazza az adott oszlop id-ját, nevét(*Title*), illetve a hozzátartozó teendőket.|
|`showDeleteModal`|A komponens által megjelenítendő szerkesztő modális ablak `show` tulajdonságának értékét kezeli.|
|`showDeleteModal`|A komponens által megjelenítendő törlő modális ablak `show` tulajdonságának értékét kezeli.|

##### `handle...`
A `handle` szóval kezdődő függvények a modális ablakok megnyitását illetve bezárását szolgálják
##### `buildTodoComponent`
A globális állapotból lekért teendőkből `Todo` komponenseket készít.
##### `deleteThisColumn`
Törli az adott oszlopot, majd frissíti a globális állapotot.
#### Todo.js
Egy teendőmegjelenítését, illetve az teendő szerkesztésére és törlésére szolgáló modális ablak renderelését és láthatóságának kezelését végzi.  `useEffect` *hook* segítségével renderelés előtt és a globális állapot változásakor a `DataContextProvider` által biztosított globális állapotból lekéri az adott teendő aktuális állapotát és eltárolja saját állapotban. Három saját állapotot tart nyilván a komponens:

|Állapot|Mit tárol|
|--|--|
|`thisTodo`|Tartalmazza az adott teendő id-ját, nevét(*Title*), leírását, határidejét, prioritását, és az őt tartalmazó oszlop id-ját.|
|`showTodoDeleteModal`|A komponens által megjelenítendő szerkesztő modális ablak `show` tulajdonságának értékét kezeli.|
|`showDeleteModal`|A komponens által megjelenítendő törlő modális ablak `show` tulajdonságának értékét kezeli.|

##### `handle...`
A `handle` szóval kezdődő függvények a modális ablakok megnyitását illetve bezárását szolgálják
##### `deleteThisTodo`
Törli az adott teendőt, majd frissíti a globális állapotot.
#### Placeholder.js
A kapott `type` *prop* értékétől függően két nagyon hasonló feladatot lát el: segítségével új oszlopot vagy új teendőt hozhat létre a felhasználó. 
A `show` állapot a megjelenítendő modális ablak láthatóságának kezelésére szolgál.
A `handleClose` és `handleShow` függvények a modális ablak láthatóságának állítására szolgál.
A `placeHolderClass` konstans értéke is a `type` értékétől függ. A komponens kinézetének beállítására szolgál, adott osztálynévhez más szabályok vonatkoznak az alkalmazás `App.css` stíluslapjában.
A modal változó tartalma szintén a type prop értékétől függ, vagy egy `TodoModal`, vagy egy `ColumnModal` komponenst kap értékül.
#### ColumnModal.js
Oszlop nevének szerkesztésére szolgál. `useEffect` hook segítségével renderelés előtt, illetve a modális ablak kezelésére használt `show` *prop* változásakor a globális állapotból lekéri és beállítja a szintén *prop*-ként kapott id alapján a `currentColumn` állapot tartalmát. 
Saját állapotok:
|Állapot|Mit tárol|
|--|--|
|`currentColumn`|Az aktuálisan szerkesztendő (vagy létrehozandó) oszlop id-ját és nevét (*title*) tárolja.|
|`validated`|A komponens által megjelenített form `validated` attribútumának értékét tárolja.|
|`showAlert`|A névütközés esetén megjelenítendő figyelmeztetés `show` attribútumának értékét tárolja.|
A `formRef` konstans a komponens által megjelenített form referenciáját tárolja, amire validációkor van szükség.

A `handleTitleChange` nevű függvény kezeli az input mezőbe beírt szöveg eltárolását állapotban.

A `createColumn` függvény megpróbál létrehozni egy új oszlopot, majd visszaadni egy tömböt , ami az aktuálisan létező oszlopokat tartalmazza kiegészítve az új oszloppal. Amennyiben nem egyéni nevet ad meg a felhasználó megjelenít egy üzenetet erről a felhasználónak, és `null`-lal tér vissza.

A `modifyColumn` függvény hasonlóan működik, azzal az eltéréssel, hogy egy oszlopot módosít. Névütközés esetén ugyanúgy viselkedik.

A `handleSaveOnBackend` függvény az alapján hívja meg vagy a `createColumn`, vagy a `modifyColumn` függvényt, hogy a `currentColumn.id` értéke *truthy*-e (azaz, hogy egy `Placeholder` renderelte ki hozzáadás, vagy egy `Column` módosítás céljából). Végül módosítja a globális állapotot a visszakapott tömbbel (vagy változatlan tömbbel `null` visszakapott érték esetén).

A `save` függvény hívódik meg a *Save Changes* felíratú gombra kattintva. Kezeli a validációt, illetve, sikeres mentés után bezárja a modális ablakot.

A modális ablak `show` attribútumának értékét , illetve a bezárást kezelő függvényt (`hide`)*prop*-ként kapja a komponens.

#### TodoModal.js
Teendő adataiak szerkesztésére szolgál. `useEffect` hook segítségével renderelés előtt, illetve a modális ablak kezelésére használt `show` *prop* változásakor a globális állapotból lekéri és beállítja a szintén *prop*-ként kapott id alapján a `currentTodo` állapot tartalmát. Amennyiben nem kap id-t, csak az oszlopazonosítót állítja be, és a jelenlegi időpontot adja értékül határidőnek.
Saját állapotok:
|Állapot|Mit tárol|
|--|--|
|`currentTodo`|Az aktuálisan szerkesztendő (vagy létrehozandó) teendő id-ját, nevét (*title*), leírását, határidejét, prioritását, és az őt tartalmazó oszlop id-ját tárolja.|
|`validated`|A komponens által megjelenített form `validated` attribútumának értékét tárolja.|
|`showAlert`|A névütközés esetén megjelenítendő figyelmeztetés `show` attribútumának értékét tárolja.|
A `formRef` konstans a komponens által megjelenített form referenciáját tárolja, amire validációkor van szükség.

A `handle...` kezdetű függvények kezelik az input mezőkbe beírt szövegek eltárolását állapotban.

A `getCurrentDateTimeString` föggvény visszatér a jelenlegi időpontot ábrázoló, a szerver által elfogadott formátumú sztringgel.

A `createTodo` függvény megpróbál létrehozni egy új teendőt, majd visszaadni egy tömböt , ami az aktuálisan létező teendőket tartalmazza kiegészítve az új teendővel. Amennyiben nem egyéni nevet ad meg a felhasználó megjelenít egy üzenetet erről a felhasználónak, és `null`-lal tér vissza.

A `modifyTodo` függvény hasonlóan működik, azzal az eltéréssel, hogy egy teendőt módosít. Névütközés esetén ugyanúgy viselkedik.

A `handleSaveOnBackend` függvény az alapján hívja meg vagy a `createTodo`, vagy a `modifyTodo` függvényt, hogy a `currentTodo.id` értéke *truthy*-e (azaz, hogy egy `Placeholder` renderelte ki hozzáadás, vagy egy `Todo` módosítás céljából). Végül módosítja a globális állapotot a visszakapott tömbbel (vagy változatlan tömbbel `null` visszakapott érték esetén).

A `save` függvény hívódik meg a *Save Changes* felíratú gombra kattintva. Kezeli a validációt, illetve, sikeres mentés után bezárja a modális ablakot.

A modális ablak `show` attribútumának értékét , illetve a bezárást kezelő függvényt (`hide`)*prop*-ként kapja a komponens.
#### DeleteModal.js
Oszlop vagy teendő törlésekor megjelenő megerősítő modális ablak. A működése erősen függ attól, hogy mely másik komponens jeleníti meg és milyen *prop*-okat kap.
|*prop*|Mire szolgál|
|--|--|
|`title`|A modális ablak szövegének előállításában van szerepe.|
|`itemType`|A modális ablak szövegének előállításában van szerepe.|
|`show`|A modális ablak *show* attribútumának értékét tartalmazza.|
|`hide`|A modális ablak eltűntetésére szolgáló függvény.|
|`delete`|A ténylegest törlést végző függvény.|


## Backend
Az alkalmazás szerveroldali része fogadja a klienstől érkező HTTP kéréseket, és objektum-relációs leképzést alkalmazva továbbítja egy Microsoft SQL Server adatbázis felé bizonyos üzleti logikai feladatok ellátása után.
### Az alkalmazás szerveroldali architektúrája
A kliens a szerverrel REST végpontokon kommunikál. Három rétegű architektúrára épült az alkalmazás szerveroldali része.
#### API réteg
Az API réteg definiálja a REST végpontokat, és a végpontok által elfogadott kéréseket, azok tulajdonságait. 
#### BLL (Business Logic Layer)
Ez a réteg alkalmazás üzleti logikai feladatait látja el. Az API rétegbe érkezett kérések validálása, és feldolgozása, azaz az adatbázisnak továbbküldés, vagy onnan történő adatlehívás a feladata.
#### DAL (Data Access Layer)
Az adatelérési réteg valósítja meg az objektum-relációs leképezést, és biztosítja az adatbáziskontextust az üzleti logikai réteg számára az adatbázissal történő kommunikáció céljából.

### Az adatbázis által tárolt entitások
#### Column
|Attribútum|Mit tárol|Adattípus|
|--|--|--|
|Id|egy oszlop elsődleges kulcsa|`int`|
|Title|egy oszlop neve|`nvarchar(MAX)`|

#### Todo
|Attribútum|Mit tárol|Adattípus|
|--|--|--|
|Id|egy teendő elsődleges kulcsa|`int`|
|Title|egy teendő neve.|`nvarchar(MAX)`|
|Description|teendő részletesebb leírása|`nvarchar(MAX)`|
|Priority|teendő prioritása|`int`|
|ColumnId|teendőt tartalmazó oszlop elsődleges kulcsa *idegen kulcs*ként|`int`|
|Deadline|teendő elvégzésének határideje|`datetime2(7)`|

### Az alkalmazást alkotó projektek felépítése
#### Todo.API
A réteg nem kezeli a valódi entitásoknak megfeleltetett osztálypéldányokat, csak az architektúrában eggyel "alsóbb" rétegben definiált DTO-k (*Data Transfer Object*) segítségével fogad, és küld adatokat.
##### `Program.cs`
Visual Studio által generált osztály, az alkalmazás belépési pontja.
##### `Startup.cs`
Visual Studio által generált osztály, az alkalmazás konfigurációját írja le. Hozzáadja a *dependency injection*-t kezelő rendszerbe az üzleti logikai réteg *service*-eit, konfigurálja az alkalmazás működéséhez szükséges *middleware*-eket.
##### `ColumnsController.cs`
A kontroller osztály a *dependency injection*-t kezelő rendszertől vár Egy `IColumnService` interfészt megvalósító osztálypéldányt, amin keresztül kommunikál az üzleti logikai réteggel. Az interfészt használó megoldás általi laza kapcsolat lehetővé teszi, hogy az osztály a függősége nélkül tesztelhető legyen.
|HTTP ige|Végpont|Feladat|
|--|--|--|
|*GET*|api/columns|összes oszlop lekérése|
|*GET*|api/columns/{id}|egyetlen oszlop lekérése|
|*POST*|api/columns|új oszlop létrehozása|
|*PUT*|api/columns/{id}|oszlop módosítása|
|*DELETE*|api/columns/{id}|oszlop törlése|
##### `TodosController.cs`
A kontroller osztály a *dependency injection*-t kezelő rendszertől vár Egy `ITodosService` interfészt megvalósító osztálypéldányt, amin keresztül kommunikál az üzleti logikai réteggel. Az interfészt használó megoldás általi laza kapcsolat lehetővé teszi, hogy az osztály a függősége nélkül tesztelhető legyen.
|HTTP ige|Végpont|Feladat|
|--|--|--|
|*GET*|api/columns|összes teendő lekérése|
|*GET*|api/columns/{id}|egyetlen teendő lekérése|
|*POST*|api/columns|új teendő létrehozása|
|*PUT*|api/columns/{id}|teendő módosítása|
|*DELETE*|api/columns/{id}|teendő törlése|
#### Todo.BLL
##### `ColumnsService.cs`
Ebben a fájlban található az `IColumnService` interfész, ami *Column* entitásokkal kapcsolatos műveleteket definiál, és annak megvalósítása, a `ColumnService` osztály. Az osztály konstruktorában vár egy `TodoDbContext` példányt. Az osztály Entity Framework és LINQ segítségével írja és olvassa az adatbázist az adatelérési rétegben definiált adatbáziskontextuson keresztül.
##### `TodosService.cs`
Ebben a fájlban található az `ITodoService` interfész, ami *Todo* entitásokkal kapcsolatos műveleteket definiál, és annak megvalósítása, a `TodoService` osztály. Az osztály konstruktorában vár egy `TodoDbContext` példányt. Az osztály Entity Framework és LINQ segítségével írja és olvassa az adatbázist az adatelérési rétegben definiált adatbáziskontextuson keresztül.
##### `DTO.cs`
Ez a fájl tartalmazza C# recrd-ként az oszlopok és teendők adatainak hálózaton történő közlekedéséhez használt *Data Transfer Object*-eket.
##### `DependencyInjectionExtensions.cs`
*Extension* metódusokat definiál az `IServiceCollection` interfészhez, amiket az API réteg `Starup` osztálya hív meg annak érdekében, hogy a saját osztályok is injektálhatók legyenek.
#### Todo.DAL
##### `Todo.cs`
A *Todo* entitást leképző osztály. Tartalmazza a a saját adatait tároló adatmezőket, illetve a *Column* entitással való kapcsolatot reprezentáló `ColumnId` külsőkulcsot tároló mezőt, és a `Column` navigációs mezőt. 
##### `Column.cs`
A *Column* entitást leképző osztály. Tartalmazza a a saját adatait tároló adatmezőket, illetve a *Todo* entitással való kapcsolatot reprezentáló `TodoId` külsőkulcsot tároló mezőt, és a `Todos` navigációs mezőt, ami az oszlophoz tartozó teendőket tárolja. 


A két  modellosztályban a kapcsolat navigációs tulajdonsággal és elsődleges kulcsot tároló mezővel is reprezentálva van, ezáltal a kapcsolat teljesen definiált. Ennek köszönhetően oszlop törlésekor a hozzátartozó teendők is törlődnek.

Ezen felül mindkét modell osztály implementálja az `IEquatable<T>` interfészt, ami lehetővé teszi oszlopok és teendők értékszerinti egyenlőség vizsgálatát. Ennekt tesztelés során van szerepe.
##### `TodoDbContext.cs`
Leszármazik az Entity Framework által rendelkezésre álló DbContext osztályból, és definiálja a két létrehozandó táblát. Az alkalmazás ezen az osztályon keresztül képes írni, olvasni az adatbázist.

### Tesztek
A *Todo.Test* projekt az alkalmazáshoz tartozó teszteket tartalmazza.
#### `ColumnsControllerUnitTests.cs`
A `ColuumnsController` osztályhoz tartozó unittestek.
#### `MockColumnService.cs`
`IColumnService` implementáció. Az oszlopokat kezelő kontrollert ezzel példányosítva, az annak valódi függősége nélkül tesztelhető.
#### `ColumnTests.cs`
Integrációs tesztek, melyek az alkalmazás mindhárom rétegét egyszerre tesztelik oszlopok kezelése szempontjából.
#### `TodoTests.cs`
Integrációs tesztek, melyek az alkalmazás mindhárom rétegét egyszerre tesztelik teendők kezelése szempontjából.
#### `TestWebbAppFactory.cs` (nem saját)
Az integrációs tesztekhez felhasznált osztály, ami a valós adatbázis helyett biztosít egySQLite adatbázist, és rajta keresztül HTTP kérések intézhetők az API réteg irányába.
