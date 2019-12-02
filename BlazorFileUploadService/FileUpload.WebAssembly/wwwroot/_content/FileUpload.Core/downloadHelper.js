window.downloadHelper = {
    downloadFile: function (data, fileName, mimeType) {
        const link = document.createElement("a");
        const blob = window.downloadHelper.base64ToBlob(data, mimeType);
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.dispatchEvent(new MouseEvent(`click`));
    },
    base64ToBlob: function (base64, mimeType) {
        const binary = atob(base64.replace(/\s/g, ""));
        const len = binary.length;
        const buffer = new ArrayBuffer(len);
        const view = new Uint8Array(buffer);

        for (let i = 0; i < len; i++) {
            view[i] = binary.charCodeAt(i);
        }

        return new Blob([view], { type: mimeType });
    }
};