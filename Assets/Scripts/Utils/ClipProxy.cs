using System;
using MoonSharp.Interpreter;

public class ClipProxy
{
    private int? index;
    private string filename;

    public ClipProxy(string filename) => this.filename = filename;

    public int Index => (int)index;
    public string Filename => filename;
    public bool IsReady() => (index != null);
    public void SetIndex(int index) => this.index = index;
}

/*

m = create_sample("/Users/parkermalachowsky/ECS_Hybrid_Sound_System_2D/Assets/Audio/jazz-guitar.wav")
m.Play(1.0)

kick = create_sample("/Users/parkermalachowsky/Downloads/fef.wav")
snare = create_sample("/Users/parkermalachowsky/Downloads/ND_Snare_01.wav")
beat = 0.5
kick.Play(beat)
kick.Play(beat*2)
snare.Play(beat*2)
kick.Play(beat*3)
kick.Play(beat*4)
snare.Play(beat*4)

kick = create_sample("/Users/parkermalachowsky/Downloads/fef.wav")
snare = create_sample("/Users/parkermalachowsky/Downloads/ND_Snare_01.wav")
beat = 0.5
for i=1,10
	do 
		kick.Play(beat*i)
		if (i % 2) == 0 then
			snare.Play(beat*i)
		end
	end

*/
