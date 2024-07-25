window.indexedDBFunctions = {
    open: async function (dbName, storeName) {
        console.log("Tentando abrir o banco de dados e criar o objeto de armazenamento...");
        let version = 1; // Certifique-se de que a versão seja maior do que a versão atual do banco de dados
        let openRequest = indexedDB.open(dbName, version);

        return new Promise((resolve, reject) => {
            openRequest.onupgradeneeded = function (e) {
                console.log("Evento onupgradeneeded acionado.");
                let db = e.target.result;
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: 'name' });
                    console.log("Objeto de armazenamento criado: " + storeName);
                }
            };

            openRequest.onsuccess = function (e) {
                console.log("Banco de dados aberto com sucesso.");
                resolve(e.target.result);
            };

            openRequest.onerror = function (e) {
                console.log("Erro ao abrir o banco de dados:", e.target.error);
                reject(e.target.error);
            };
        });
    },
    add: async function (dbName, storeName, assembly) {
        console.log("Parâmetros recebidos para adicionar ao banco de dados:");
        console.log(`dbName: ${dbName}`);
        console.log(`storeName: ${storeName}`);
        console.log("assembly:", assembly);

        try {
            let db = await this.open(dbName, storeName);
            let transaction = db.transaction(storeName, 'readwrite');
            let store = transaction.objectStore(storeName);
            let request = store.add(assembly);

            await new Promise((resolve, reject) => {
                request.onsuccess = function (e) {
                    resolve();
                };

                request.onerror = function (e) {
                    reject(e.target.error);
                };
            });
        } catch (error) {
            console.error("Erro ao adicionar ao banco de dados:", error);
            throw new Error("Erro ao adicionar ao banco de dados: " + error.message);
        }
    },
    remove: async function (dbName, storeName, assemblyName) {
        console.log("Parâmetros recebidos para remover do banco de dados:");
        console.log(`dbName: ${dbName}`);
        console.log(`storeName: ${storeName}`);
        console.log(`assemblyName: ${assemblyName}`);

        try {
            let db = await this.open(dbName, storeName);
            let transaction = db.transaction(storeName, 'readwrite');
            let store = transaction.objectStore(storeName);
            let request = store.delete(assemblyName);

            await new Promise((resolve, reject) => {
                request.onsuccess = function (e) {
                    resolve();
                };

                request.onerror = function (e) {
                    reject(e.target.error);
                };
            });
        } catch (error) {
            console.error("Erro ao remover do banco de dados:", error);
            throw new Error("Erro ao remover do banco de dados: " + error.message);
        }
    },
    getAll: async function (dbName, storeName) {
        try {
            console.log("Parâmetros recebidos para adicionar ao banco de dados:");
            console.log(`dbName: ${dbName}`);
            console.log(`storeName: ${storeName}`);
            let db = await this.open(dbName, storeName);
            let transaction = db.transaction(storeName, 'readonly');
            let store = transaction.objectStore(storeName);
            let request = store.getAll();

            let result = await new Promise((resolve, reject) => {
                request.onsuccess = function (e) {
                    resolve(e.target.result);
                };

                request.onerror = function (e) {
                    reject(e.target.error);
                };
            });

            console.log(result);
            return result;
        } catch (error) {
            console.error("Erro ao recuperar todos os registros:", error);
            throw new Error("Erro ao recuperar todos os registros: " + error.message);
        }
    }
};
