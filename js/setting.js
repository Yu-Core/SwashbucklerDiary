export function readSettings(keys) {
    var allItems = {};
    keys.forEach(key => {
        if (localStorage.hasOwnProperty(key)) {
            allItems[key] = JSON.parse(localStorage[key]);
        }
    })
    
    return JSON.stringify(allItems);
}
