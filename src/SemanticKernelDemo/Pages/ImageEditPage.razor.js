export function init(container) {

    //const img = container.querySelector('img');
    const img = document.getElementById("img-container")
    if (img.src && img.complete) {
        attachPaintArea(img);
    } else {
        img.onload = () => attachPaintArea(img);
    }
}

export function clearSelection(container) {
    const canvas = container.querySelector('canvas');
    const ctx = canvas.ctx;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
}

function attachPaintArea(img) {
    img.onload = null;
    const canvas = document.createElement('canvas');
    canvas.width = img.width;
    canvas.height = img.height;
    img.parentNode.appendChild(canvas);
    canvas.ctx = canvas.getContext('2d');
    canvas.ctx.sourceImage = img;

    canvas.addEventListener('mousedown', onDrawStart);
    canvas.addEventListener('mousemove', onDrawMove);
    canvas.addEventListener('mouseup', onDrawEnd);

    canvas.addEventListener('touchstart', onTouchDrawStart);
    canvas.addEventListener('touchmove', onTouchDrawMove);
    canvas.addEventListener('touchend', onTouchDrawEnd);
}

function getTouchPos(canvasDom, touchEvent) {
    var rect = canvasDom.getBoundingClientRect();
    return {
        x: touchEvent.touches[0].clientX - rect.left,
        y: touchEvent.touches[0].clientY - rect.top
    };
}

function onTouchDrawStart(e) {
    const canvas = this;
    var touch = getTouchPos(canvas,e);
    canvas.width = canvas.ctx.sourceImage.width;
    canvas.height = canvas.ctx.sourceImage.height;
    canvas.currentPath = [{ x: touch.x, y: touch.y }];
}

function onTouchDrawMove(e) {
    const canvas = this;
    if (!canvas.currentPath)
        return;
    var touch = getTouchPos(canvas, e);
    const newPoint = { x: touch.x, y: touch.y };
    const prevPoint = canvas.currentPath[canvas.currentPath.length - 1];
    const distanceSquared = (newPoint.x - prevPoint.x) * (newPoint.x - prevPoint.x) + (newPoint.y - prevPoint.y) * (newPoint.y - prevPoint.y);
    if (distanceSquared > 9) {
        canvas.currentPath.push(newPoint);
        drawCurrentPath(canvas);
    }
}

async function onTouchDrawEnd(e) {
    const canvas = this;
    const ctx = canvas.ctx;
    //var touch = getTouchPos(canvas, e);
    // Get source image bytes
    ctx.drawImage(ctx.sourceImage, 0, 0, ctx.sourceImage.width, ctx.sourceImage.height);
    const imageArrayBuffer = await getCanvasDataAsync(canvas);

    // Get mask image bytes
    drawCurrentPath(canvas);
    const selectionArrayBuffer = await getCanvasDataAsync(canvas);
    canvas.currentPath = null;
    var img1 = new Object();
    var img2 = new Object();
    try {
        img1 = returnArray2(imageArrayBuffer);
        img2 = returnArray2(selectionArrayBuffer);
    } catch (error) {
        console.log(error);
        alert(error);
    }
    //alert('hey kamu');

    /*
    var data1 = returnArray(imageArrayBuffer);//DotNet.createJSObjectReference(readable(imageArrayBuffer));
    var data2 = returnArray(selectionArrayBuffer);//DotNet.createJSObjectReference(readable(selectionArrayBuffer));
    DotNet.invokeMethodAsync('SemanticKernelDemo', 'GetSelectedRegion', data1, data2)
        .then(data => {
            console.log(data);
        });
    */
    // Raise custom event

    canvas.dispatchEvent(new CustomEvent('regiondrawn', {
        bubbles: true,
        detail: {
            // These properties are JSON-serialized and become the event args on the .NET side
            sourceImage: img1,//DotNet.createJSObjectReference(readable(imageArrayBuffer)),
            selectedRegion: img2//DotNet.createJSObjectReference(readable(selectionArrayBuffer)),
        }
    }));
}

function onDrawStart(evt) {
    const canvas = this;
    canvas.width = canvas.ctx.sourceImage.width;
    canvas.height = canvas.ctx.sourceImage.height;
    canvas.currentPath = [{ x: evt.offsetX, y: evt.offsetY }];
}

function onDrawMove(evt) {
    const canvas = this;
    if (!canvas.currentPath)
        return;
    const newPoint = { x: evt.offsetX, y: evt.offsetY };
    const prevPoint = canvas.currentPath[canvas.currentPath.length - 1];
    const distanceSquared = (newPoint.x - prevPoint.x) * (newPoint.x - prevPoint.x) + (newPoint.y - prevPoint.y) * (newPoint.y - prevPoint.y);
    if (distanceSquared > 9) {
        canvas.currentPath.push(newPoint);
        drawCurrentPath(canvas);
    }
}

async function onDrawEnd(evt) {
    const canvas = this;
    const ctx = canvas.ctx;

    // Get source image bytes
    ctx.drawImage(ctx.sourceImage, 0, 0, ctx.sourceImage.width, ctx.sourceImage.height);
    const imageArrayBuffer = await getCanvasDataAsync(canvas);

    // Get mask image bytes
    drawCurrentPath(canvas);
    const selectionArrayBuffer = await getCanvasDataAsync(canvas);
    canvas.currentPath = null;
    var img1 = new Object();
    var img2 = new Object();
    try {
        img1 = returnArray2(imageArrayBuffer);
        img2 = returnArray2(selectionArrayBuffer);
    } catch (error) {
        console.log(error);
        alert(error);
    }
    
    //alert('hey kamu');
   

    /*
    var data1 = returnArray(imageArrayBuffer);//DotNet.createJSObjectReference(readable(imageArrayBuffer));
    var data2 = returnArray(selectionArrayBuffer);//DotNet.createJSObjectReference(readable(selectionArrayBuffer));
    DotNet.invokeMethodAsync('SemanticKernelDemo', 'GetSelectedRegion', data1, data2)
        .then(data => {
            console.log(data);
        });
    */
    // Raise custom event
    
    canvas.dispatchEvent(new CustomEvent('regiondrawn', {
        bubbles: true,
        detail: {
            // These properties are JSON-serialized and become the event args on the .NET side
            sourceImage: img1,//DotNet.createJSObjectReference(readable(imageArrayBuffer)),
            selectedRegion: img2 //DotNet.createJSObjectReference(readable(selectionArrayBuffer)),
        }
    }));
}

function getCanvasDataAsync(canvas) {
    return new Promise(resolve => {
        canvas.toBlob(result => resolve(result.arrayBuffer()), 'image/png');
    });
}

function drawCurrentPath(canvas) {
    const ctx = canvas.ctx;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.beginPath();
    ctx.fillStyle = 'rgba(255,0,0,0.4)';
    ctx.moveTo(canvas.currentPath[0].x, canvas.currentPath[0].y);
    for (var i = 1; i < canvas.currentPath.length; i++) {
        ctx.lineTo(canvas.currentPath[i].x, canvas.currentPath[i].y);
    }
    ctx.closePath();
    ctx.fill();

    ctx.setLineDash([3, 6]);
    ctx.strokeStyle = 'black';
    ctx.stroke();
}

function returnArray(arrayBuffer) {
    var myUint8Array = new Uint8Array(arrayBuffer);
    return btoa(String.fromCharCode.apply(null, myUint8Array ));  
    
}
function returnArray2(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
}

function readable(arrayBuffer) {
    return {
        getBytes: () => BINDING.js_typed_array_to_array(new Uint8Array(arrayBuffer))
    };
}

Blazor.registerCustomEventType('regiondrawn', { createEventArgs: event => event.detail });
