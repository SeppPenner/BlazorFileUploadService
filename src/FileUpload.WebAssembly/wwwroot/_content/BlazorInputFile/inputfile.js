(function () {
    window.BlazorInputFile = {
        init: function (elem, componentInstance) {
            var nextFileId = 0;

            elem.addEventListener("change", function () {
                // Reduce to purely serializable data, plus build an index by ID
                elem._blazorFilesById = {};
                const fileList = Array.prototype.map.call(elem.files, function (file) {
                    const result = {
                        id: ++nextFileId,
                        lastModified: new Date(file.lastModified).toISOString(),
                        name: file.name,
                        size: file.size,
                        type: file.type
                    };
                    elem._blazorFilesById[result.id] = result;

                    // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                    Object.defineProperty(result, "blob", { value: file });

                    return result;
                });

                componentInstance.invokeMethodAsync("NotifyChange", fileList).then(null, function (err) {
                    throw new Error(err);
                });
            });
        },

        readFileData: function (elem, fileId, startOffset, count) {
            const readPromise = getArrayBufferFromFileAsync(elem, fileId);

            return readPromise.then(function (arrayBuffer) {
                const uint8Array = new Uint8Array(arrayBuffer, startOffset, count);
                const base64 = window.uint8ToBase64(uint8Array);
                return base64;
            });
        },

        ensureArrayBufferReadyForSharedMemoryInterop: function (elem, fileId) {
            return getArrayBufferFromFileAsync(elem, fileId).then(function (arrayBuffer) {
                getFileById(elem, fileId).arrayBuffer = arrayBuffer;
            });
        },

        readFileDataSharedMemory: function (readRequest) {
            // This uses various unsupported internal APIs. Beware that if you also use them,
            // your code could become broken by any update.
            // ReSharper disable UseOfImplicitGlobalInFunctionScope
            // ReSharper disable PossiblyUnassignedProperty
            const inputFileElementReferenceId = Blazor.platform.readStringField(readRequest, 0);
            const inputFileElement = document.querySelector(`[_bl_${inputFileElementReferenceId}]`);
            const fileId = Blazor.platform.readInt32Field(readRequest, 4);
            const sourceOffset = Blazor.platform.readUint64Field(readRequest, 8);
            const destination = Blazor.platform.readInt32Field(readRequest, 16);
            const destinationOffset = Blazor.platform.readInt32Field(readRequest, 20);
            const maxBytes = Blazor.platform.readInt32Field(readRequest, 24);

            const sourceArrayBuffer = getFileById(inputFileElement, fileId).arrayBuffer;
            const bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength - sourceOffset);
            const sourceUint8Array = new Uint8Array(sourceArrayBuffer, sourceOffset, bytesToRead);

            const destinationUint8Array = Blazor.platform.toUint8Array(destination);
            destinationUint8Array.set(sourceUint8Array, destinationOffset);

            return bytesToRead;
        }
    };

    function getFileById(elem, fileId) {
        const file = elem._blazorFilesById[fileId];
        if (!file) {
            throw new Error(`There is no file with ID ${fileId}. The file list may have changed`);
        }

        return file;
    }

    function getArrayBufferFromFileAsync(elem, fileId) {
        var file = getFileById(elem, fileId);

        // On the first read, convert the FileReader into a Promise<ArrayBuffer>
        if (!file.readPromise) {
            file.readPromise = new Promise(function (resolve, reject) {
                var reader = new FileReader();
                reader.onload = function () { resolve(reader.result); };
                reader.onerror = function (err) { reject(err); };
                reader.readAsArrayBuffer(file.blob);
            });
        }

        return file.readPromise;
    }

    window.uint8ToBase64 = (function () {
        // Code from https://github.com/beatgammit/base64-js/
        // License: MIT
        var lookup = [];

        const code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        for (var i = 0, len = code.length; i < len; ++i) {
            lookup[i] = code[i];
        }

        function tripletToBase64(num) {
            return lookup[num >> 18 & 0x3F] +
                lookup[num >> 12 & 0x3F] +
                lookup[num >> 6 & 0x3F] +
                lookup[num & 0x3F];
        }

        function encodeChunk(uint8, start, end) {
            var tmp;
            const output = [];
            for (let j = start; j < end; j += 3) {
                tmp =
                    ((uint8[j] << 16) & 0xFF0000) +
                    ((uint8[j + 1] << 8) & 0xFF00) +
                    (uint8[j + 2] & 0xFF);
                output.push(tripletToBase64(tmp));
            }
            return output.join("");
        }

        return function (uint8) {
            var tmp;
            const length = uint8.length;
            const extraBytes = length % 3; // if we have 1 byte left, pad 2 bytes
            const parts = [];
            const maxChunkLength = 16383; // must be multiple of 3

            // go through the array every three bytes, we'll deal with trailing stuff later
            for (var i = 0, len2 = length - extraBytes; i < len2; i += maxChunkLength) {
                parts.push(encodeChunk(
                    uint8, i, (i + maxChunkLength) > len2 ? len2 : (i + maxChunkLength)
                ));
            }

            // pad the end with zeros, but make sure to not forget the extra bytes
            if (extraBytes === 1) {
                tmp = uint8[length - 1];
                parts.push(
                    lookup[tmp >> 2] +
                    lookup[(tmp << 4) & 0x3F] +
                    "=="
                );
            } else if (extraBytes === 2) {
                tmp = (uint8[length - 2] << 8) + uint8[length - 1];
                parts.push(
                    lookup[tmp >> 10] +
                    lookup[(tmp >> 4) & 0x3F] +
                    lookup[(tmp << 2) & 0x3F] +
                    "="
                );
            }

            return parts.join("");
        };
    })();
})();
