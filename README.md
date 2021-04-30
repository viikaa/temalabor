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
Egy oszlop megjelenítését, illetve az oszlop szerkesztésére és törlésére szolgáló modális ablak renderelését és láthatóságának kezelését végzi.  `useEffect` *hook* segítségével renderelés előtt a `DataContextProvider` által biztosított globális állapotból lekéri a hozzátartozó teendőket és eltárolja saját állapotban. Három saját állapotot tart nyilván a komponens:
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
#### Placeholder.js
#### ColumnModal.js
#### TodoModal.js
#### DeleteModal.js




## Backend

