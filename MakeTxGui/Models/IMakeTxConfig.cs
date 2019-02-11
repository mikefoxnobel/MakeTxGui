using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeTxGui.Models
{
    /*
        --help	Print help message
        -v	Verbose status messages
        -o %s	Output filename
        --threads %d	Number of threads (default: #cores)
        -u	Update mode
        --format %s	Specify output file format (default: guess from extension)
        --nchannels %d	Specify the number of output image channels.
        -d %s	Set the output data format to one of: uint8, sint8, uint16, sint16, half, float
        --tile %d %d	Specify tile size
        --separate	Use planarconfig separate (default: contiguous)
        --fov %f	Field of view for envcube/shadcube/twofish
        --fovcot %f	Override the frame aspect ratio. Default is width/height.
        --wrap %s	Specify wrap mode (black, clamp, periodic, mirror)
        --swrap %s	Specifiy s wrap mode separately
        --twrap %s	Specifiy t wrap mode separately
        --resize	Resize textures to power of 2 (default: no)
        --noresize	Do not resize textures to power of 2 (deprecated)
        --filter %s	Select filter for resizing (choices: box triangle gaussian catrom blackman-harris sinc lanczos3 radial-lanczos3 mitchell bspline, disk, default=box)
        --nomipmap	
        Do not make multiple MIP-map levels 

        --checknan	Check for NaN/Inf values (abort if found).
        --Mcamera %f %f %f %f %f %f %f %f %f %f %f %f %f %f %f %f	Set the camera matrix
        --Mscreen %f %f %f %f %f %f %f %f %f %f %f %f %f %f %f %f	Set the camera matrix
        --hash	Embed SHA-1 hash of pixels in the header (deprecated. hashes are always computed).
        --prman-metadata	Add prman specific metadata
        --constant-color-detect	Create 1-tile textures from constant color inputs
        --monochrome-detect	Create 1-channel textures from monochrome inputs
        --opaque-detect	Drop alpha channel that is always 1.0
        --stats	Print runtime statistics
        --mipimage %s	Specify an individual MIP level
        Basic modes (default is plain texture)	 
        --shadow	Create shadow map
        --envlatl	Create lat/long environment map
        --envcube	Create cubic env map (file order: px, nx, py, ny, pz, nz) (UNIMP)
        Color Management Options	 
        --colorconvert %s %s
        Apply a color space conversion to the image. If the output color space is not the same bit depth as input color space, it is your responsibility to set the data format to the proper bit depth using the -d option. (choices: linear, sRGB, Rec709)
        --unpremult 
        Unpremultiply before color conversion, then premultiply after the color conversion. You'll probably want to use this flag if your image contains an alpha channel.
        Configuration Presets	 
        --oiio	Use OIIO-optimized settings for tile size, planarconfig, metadata, and constant-color optimizations.
        --prman	Use PRMan-safe settings for tile size, planarconfig, and metadata.
     */
    public class MakeTxConfig
    {
        string Path { get; set; } = "";
        bool? _v { get; set; } = null;
        string _d { get; set; } = "";
        int? __threads { get; set; } = null;
        bool? _u { get; set; } = null;
        string __format { get; set; } = "";
        int? __nchannerls { get; set; } = null;
        int[] __tile { get; set; } = null;
        bool? __separate { get; set; } = null;
        float? __fov { get; set; } = null;
        float? __fovcot { get; set; } = null;
        string __wrap { get; set; } = "";
        string __swrap { get; set; } = "";
        string __twrap { get; set; } = "";
        bool? __resize { get; set; } = null;
        bool? __noresize { get; set; } = null;
        string __filter { get; set; } = "";
        bool? __nomipmap { get; set; } = null;
        bool? __checknan { get; set; } = null;
        float[] __Mcamera { get; set; } = null;
        float[] __Mscreen { get; set; } = null;
        bool? __ash { get; set; } = null;
        bool? __prman_metadata { get; set; } = null;
        bool? __constant_color_detect { get; set; } = null;
        bool? __monochrome_detect { get; set; } = null;
        bool? __opaque_detect { get; set; } = null;
        bool? __stats { get; set; } = null;
        string __mipimage { get; set; } = "";

        bool? __shadow { get; set; } = null;
        bool? __envlatl { get; set; } = null;
        bool? __envcube { get; set; } = null;

        string[] __colorconvert { get; set; }
        bool? __unpremult { get; set; } = null;

        bool? __oiio { get; set; } = null;
        bool? __prman { get; set; } = null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Path);
            sb.Append(_v.HasValue ? " -v" : "");
            sb.Append(_d != String.Empty ? String.Format(" -d {0}", _d) : "");
            sb.Append(__threads.HasValue ? String.Format(" --threads {0}", __threads) : "");
            sb.Append(_u.HasValue ? " -u" : "");
            sb.Append(__format != String.Empty ? String.Format(" --format {0}", __format) : "");
            sb.Append(__nchannerls.HasValue ? String.Format(" --nchannerls {0}", __nchannerls) : "");


            if (__tile != null && __tile.Length == 2)
            {
                sb.Append(" --tile");
                foreach (int value in __tile)
                {
                    sb.AppendFormat(" {0}", value);
                }
            }

            sb.Append(__separate.HasValue ? " --separate" : "");
            sb.Append(__fov.HasValue ? String.Format(" --fov {0}", __fov) : "");
            sb.Append(__fovcot.HasValue ? String.Format(" --fovcot {0}", __fovcot) : "");
            sb.Append(__wrap != String.Empty ? String.Format(" --wrap {0}", __wrap) : "");
            sb.Append(__swrap != String.Empty ? String.Format(" --swrap {0}", __swrap) : "");
            sb.Append(__twrap != String.Empty ? String.Format(" --twrap {0}", __twrap) : "");
            sb.Append(__resize.HasValue ? " --resize" : "");
            sb.Append(__noresize.HasValue ? " --noresize" : "");
            sb.Append(__filter != String.Empty ? String.Format(" --filter {0}", __filter) : "");
            sb.Append(__nomipmap.HasValue ? " --nomipmap" : "");
            sb.Append(__checknan.HasValue ? " --checknan" : "");

            if (__Mcamera != null && __Mcamera.Length == 16)
            {
                sb.Append(" --Mcamera");
                foreach (float value in __Mcamera)
                {
                    sb.AppendFormat(" {0}", value);
                }
            }

            if (__Mscreen != null && __Mscreen.Length == 16)
            {
                sb.Append(" --Mscreen");
                foreach (float value in __Mscreen)
                {
                    sb.AppendFormat(" {0}", value);
                }
            }

            sb.Append(__ash.HasValue ? " --ash" : "");
            sb.Append(__prman_metadata.HasValue ? " --prman-metadata" : "");
            sb.Append(__constant_color_detect.HasValue ? " --constant-color-detect" : "");
            sb.Append(__monochrome_detect.HasValue ? " --monochrome-detect" : "");
            sb.Append(__opaque_detect.HasValue ? " --opaque-detect" : "");
            sb.Append(__stats.HasValue ? " --stats" : "");
            sb.Append(__mipimage != String.Empty ? String.Format(" --mipimage {0}", __mipimage) : "");
            sb.Append(__shadow.HasValue ? " --shadow" : "");
            sb.Append(__envlatl.HasValue ? " --envlatl" : "");
            sb.Append(__envcube.HasValue ? " --envcube" : "");

            if (__colorconvert != null && __colorconvert.Length == 2)
            {
                sb.Append(" --colorconvert");
                foreach (string value in __colorconvert)
                {
                    sb.AppendFormat(" {0}", value);
                }
            }

            sb.Append(__unpremult.HasValue ? " --unpremult" : "");
            sb.Append(__oiio.HasValue ? " --oiio" : "");
            sb.Append(__prman.HasValue ? " --prman" : "");

            return sb.ToString();
        }
    }
}
