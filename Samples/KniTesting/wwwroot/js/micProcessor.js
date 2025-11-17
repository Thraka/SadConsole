//   micProcessor.js
class MicProcessor extends AudioWorkletProcessor
{
    constructor()
    {
        super();

        // global variables for testing
        var sampleRate = globalThis.sampleRate;
        var currentFrame = globalThis.currentFrame;
        var currentTime = globalThis.currentTime;
        var currentRenderQuantum = globalThis.currentRenderQuantum;
                
        this.SampleRate = sampleRate;
        this.TargetSamples = Math.floor(this.SampleRate * 0.1); // 100ms
        this.Buffer = new Float32Array(this.TargetSamples);
        this.BufferIndex = 0;

        this.port.onmessage = (event) =>
        {
            var data = event.data;

            if (typeof data === 'number')
            {
                //this.port.postMessage(data); // echo back test
            }
            if (data instanceof Uint8Array)
            {
            }
        };
    }

    process(inputs, outputs, parameters)
    {
        var inChannel0 = inputs[0][0];
        if (!inChannel0) return true;
                
        let srcIndex = 0;
        var srcLen = inChannel0.length;

        while (srcIndex < srcLen)
        {
            var remaining = this.TargetSamples - this.BufferIndex;
            var copyCount = Math.min(remaining, srcLen - srcIndex);

            this.Buffer.set(
                inChannel0.subarray(srcIndex, srcIndex + copyCount),
                this.BufferIndex);

            this.BufferIndex += copyCount;
            srcIndex += copyCount;

            if (this.BufferIndex >= this.TargetSamples)
            {
                this.SendBuffer();
                this.BufferIndex = 0;
            }
        }

        return true;
    }

    SendBuffer()
    {
        // convert to 16-6bit PCM
        var int16 = new Int16Array(this.TargetSamples);
        for (var i = 0; i < this.TargetSamples; i++)
        {
            int16[i] = this.Buffer[i] * 32767;
        }

        var byteArray = new Uint8Array(int16.buffer);
        this.port.postMessage(byteArray, [byteArray.buffer]);
    }
}

registerProcessor('mic-processor', MicProcessor);