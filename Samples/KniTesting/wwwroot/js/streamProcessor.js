//   streamProcessor.js
class StreamProcessor extends AudioWorkletProcessor
{
    constructor()
    {
        super();
        this.queue = [];

        this.port.onmessage = (event) =>
        {
            var data = event.data;

            if (typeof data === 'number')
            {
                if (data === 2)
                {                    
                    this.queue = [];
                }
            }
            if (data instanceof Uint8Array)
            {
                const buffer = new Int16Array(data.buffer, data.byteOffset, data.length / 2);
                buffer.offset = 0;
                this.queue.push(buffer);
            }
        };
    }


    process(inputs, outputs, parameters)
    {
        const output = outputs[0];
        
        const channelCount = output.length;
        const sampleCount = output[0].length;

        let written = 0;

        while (written < sampleCount && this.queue.length > 0)
        {
            const buffer = this.queue[0];
            const offset = buffer.offset;

            const available = buffer.length - offset;
            const needed = sampleCount - written;
            const copyCount = Math.min(available, needed);

            for (let i = 0; i < copyCount; i++)
            {
                for (let c = 0; c < channelCount; c++)
                {
                    const channel = output[c];
                    let value = (buffer[offset+i] / 32767);
                    channel[written+i] = value;
                }
            }
            
            written += copyCount;
            buffer.offset += copyCount;

            if (buffer.offset >= buffer.length)
            {
                this.queue.shift();
                this.port.postMessage(1);
            }
        }
        
        // Fill remaining samples with silence
        if (written < sampleCount)
        {
            for (let c = 0; c < channelCount; c++)
            {
                const channel = output[c];

                for (let i = written; i < sampleCount; i++)
                {
                    let value = 0;
                    channel[i] = value;
                }
            }
        }

        return true;
    }
}

registerProcessor("stream-processor", StreamProcessor);