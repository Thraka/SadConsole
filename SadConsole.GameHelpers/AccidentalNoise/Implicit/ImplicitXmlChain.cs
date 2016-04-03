using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public class ImplicitXmlChain : ImplicitModuleBase
    {
        public static ImplicitXmlChain FromString(String text)
        {
            var chain = new ImplicitXmlChain();
            var xml = XDocument.Parse(text);

            chain.Modules = new Dictionary<String, ImplicitModuleBase>();

            foreach (var xElement in xml.Descendants("module"))
            {
                var nameAttribute = xElement.Attribute("name");
                var typeAttribute = xElement.Attribute("type");

                if (nameAttribute == null) continue;
                if (typeAttribute == null) continue;

                var name = nameAttribute.Value;
                var type = typeAttribute.Value;

                switch (type)
                {
                    case "auto_correct": chain.Modules.Add(name, AutoCorrectFromXElement(chain, xElement)); break;
                    case "bias": chain.Modules.Add(name, BiasFromXElement(chain, xElement)); break;
                    case "blend": throw new NotImplementedException("blend");
                    case "bright_contrast": throw new NotImplementedException("bright_contrast");
                    case "cache": chain.Modules.Add(name, CacheFromXElement(chain, xElement)); break;
                    case "ceiling": throw new NotImplementedException("ceiling");
                    case "clamp": chain.Modules.Add(name, ClampFromXElement(chain, xElement)); break;
                    case "combiner": chain.Modules.Add(name, CombinerFromXElement(chain, xElement)); break;
                    case "constant": chain.Modules.Add(name, ConstantFromXElement(chain, xElement)); break;
                    case "cos": throw new NotImplementedException("cos");
                    case "floor": throw new NotImplementedException("floor");
                    case "fractal": chain.Modules.Add(name, FractalFromXElement(chain, xElement)); break;
                    case "gain": throw new NotImplementedException("gain");
                    case "gradient": chain.Modules.Add(name, GradientFromXElement(chain, xElement)); break;
                    case "invert": throw new NotImplementedException("invert");
                    case "log": throw new NotImplementedException("log");
                    case "pow": throw new NotImplementedException("pow");
                    case "rotate_domain": throw new NotImplementedException("rotate_domain");
                    case "scale_domain": chain.Modules.Add(name, ScaleDomainFromXElement(chain, xElement)); break;
                    case "scale_offset": chain.Modules.Add(name, ScaleOffsetFromXElement(chain, xElement)); break;
                    case "select": chain.Modules.Add(name, SelectFromXElement(chain, xElement)); break;
                    case "sin": throw new NotImplementedException("sin");
                    case "sphere": chain.Modules.Add(name, SphereFromXElement(chain, xElement)); break;
                    case "tan": throw new NotImplementedException("tan");
                    case "tiers": throw new NotImplementedException("tiers");
                    case "translate_domain": chain.Modules.Add(name, TranslateDomainFromXElement(chain, xElement)); break;
                    default: throw new NotImplementedException(type);
                }
            }

            chain.Source = chain.Modules[xml.Descendants("chain").Single().Attribute("source").Value];

            return chain;
        }

        public static ImplicitAutoCorrect AutoCorrectFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var low = (xElement.Attribute("low") != null ? xElement.Attribute("low").Value : String.Empty);
            var high = (xElement.Attribute("high") != null ? xElement.Attribute("high").Value : String.Empty);

            ImplicitAutoCorrect autoCorrect;

            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect = new ImplicitAutoCorrect(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect = new ImplicitAutoCorrect(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(low))
            {
                if (Double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!String.IsNullOrEmpty(high))
            {
                if (Double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            return autoCorrect;
        }

        public static ImplicitBias BiasFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var low = (xElement.Attribute("low") != null ? xElement.Attribute("low").Value : String.Empty);

            ImplicitBias autoCorrect;

            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect = new ImplicitBias(source, 0);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect = new ImplicitBias(value, 0);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    autoCorrect.Bias = source;
                else if (Double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    autoCorrect.Bias = value;
                else
                    throw new InvalidOperationException("Invalid bias value");
            }

            return autoCorrect;
        }

        public static ImplicitCache CacheFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);

            if (String.IsNullOrEmpty(sourceString))
                throw new InvalidOperationException("Missing source");

            ImplicitModuleBase source;
            Double value;

            if (chain.Modules.TryGetValue(sourceString, out source))
                return new ImplicitCache(source);

            if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return new ImplicitCache(value);

            throw new InvalidOperationException("Invalid source value");
        }

        public static ImplicitClamp ClampFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var low = (xElement.Attribute("low") != null ? xElement.Attribute("low").Value : String.Empty);
            var high = (xElement.Attribute("high") != null ? xElement.Attribute("high").Value : String.Empty);

            ImplicitClamp clamp;

            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    clamp = new ImplicitClamp(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp = new ImplicitClamp(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(low, out source))
                    clamp.Low = source;
                else if (Double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!String.IsNullOrEmpty(high))
            {
                if (chain.Modules.TryGetValue(high, out source))
                    clamp.High = source;
                else if (Double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    clamp.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            return clamp;
        }

        public static ImplicitConstant ConstantFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var value = xElement.Attribute("value");
            return new ImplicitConstant((value == null ? 0.00 : Double.Parse(value.Value, NumberStyles.Any, CultureInfo.InvariantCulture)));
        }

        public static ImplicitCombiner CombinerFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var combinerTypeString = (xElement.Attribute("combiner_type") != null ? xElement.Attribute("combiner_type").Value : String.Empty);

            ImplicitCombiner combiner;

            switch (combinerTypeString.ToLower())
            {
                case "add":
                    combiner = new ImplicitCombiner(CombinerType.Add);
                    break;
                case "average":
                    combiner = new ImplicitCombiner(CombinerType.Average);
                    break;
                case "max":
                    combiner = new ImplicitCombiner(CombinerType.Max);
                    break;
                case "min":
                    combiner = new ImplicitCombiner(CombinerType.Min);
                    break;
                case "multiply":
                    combiner = new ImplicitCombiner(CombinerType.Multiply);
                    break;
                default: throw new InvalidOperationException("Invalid combiner_type.");
            }

            foreach (var source in xElement.Elements("source"))
            {
                combiner.AddSource(chain.Modules[source.Value]);
            }

            return combiner;
        }

        public static ImplicitFractal FractalFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var fractalTypeAttribute = xElement.Attribute("fractal");
            if (fractalTypeAttribute == null)
                throw new InvalidOperationException("Missing fractal.");
            var basisTypeAttribute = xElement.Attribute("basis");
            if (basisTypeAttribute == null)
                throw new InvalidOperationException("Missing basis.");
            var interpolationTypeAttribute = xElement.Attribute("interpolation");
            if (interpolationTypeAttribute == null)
                throw new InvalidOperationException("Missing interpolation.");

            FractalType fractalType;
            switch (fractalTypeAttribute.Value.ToLower())
            {
                case "billow":
                    fractalType = FractalType.Billow;
                    break;
                case "fbm":
                    fractalType = FractalType.FractionalBrownianMotion;
                    break;
                case "hybrid_multi":
                    fractalType = FractalType.HybridMulti;
                    break;
                case "multi":
                    fractalType = FractalType.Multi;
                    break;
                case "ridged_multi":
                    fractalType = FractalType.RidgedMulti;
                    break;
                default: throw new InvalidOperationException("Invalid fractal.");
            }

            BasisType basisType;
            switch (basisTypeAttribute.Value.ToLower())
            {
                case "gradient":
                    basisType = BasisType.Gradient;
                    break;
                case "gradient_value":
                    basisType = BasisType.GradientValue;
                    break;
                case "simplex":
                    basisType = BasisType.Simplex;
                    break;
                case "value":
                    basisType = BasisType.Value;
                    break;
                case "white":
                    basisType = BasisType.White;
                    break;
                default: throw new InvalidOperationException("Invalid basis.");
            }

            InterpolationType interpolationType;
            switch (interpolationTypeAttribute.Value.ToLower())
            {
                case "cubic":
                    interpolationType = InterpolationType.Cubic;
                    break;
                case "linear":
                    interpolationType = InterpolationType.Linear;
                    break;
                case "none":
                    interpolationType = InterpolationType.None;
                    break;
                case "quintic":
                    interpolationType = InterpolationType.Quintic;
                    break;
                default: throw new InvalidOperationException("Invalid interpolation.");
            }

            var fractal = new ImplicitFractal(fractalType, basisType, interpolationType);

            var octavesAttribute = xElement.Attribute("octaves");
            if (octavesAttribute != null)
                fractal.Octaves = Int32.Parse(octavesAttribute.Value);

            var frequencyAttribute = xElement.Attribute("frequency");
            if (frequencyAttribute != null)
                fractal.Frequency = Double.Parse(frequencyAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var lacunarityAttribute = xElement.Attribute("lacunarity");
            if (lacunarityAttribute != null)
                fractal.Lacunarity = Double.Parse(lacunarityAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var gainAttribute = xElement.Attribute("gain");
            if (gainAttribute != null)
                fractal.Gain = Double.Parse(gainAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var offsetAttribute = xElement.Attribute("offset");
            if (offsetAttribute != null)
                fractal.Offset = Double.Parse(offsetAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            var hAttribute = xElement.Attribute("h");
            if (hAttribute != null)
                fractal.H = Double.Parse(hAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

            return fractal;
        }

        public static ImplicitGradient GradientFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var x0 = xElement.Attribute("x0");
            var x1 = xElement.Attribute("x1");
            var y0 = xElement.Attribute("y0");
            var y1 = xElement.Attribute("y1");
            var z0 = xElement.Attribute("z0");
            var z1 = xElement.Attribute("z1");
            var v0 = xElement.Attribute("v0");
            var v1 = xElement.Attribute("v1");
            var u0 = xElement.Attribute("u0");
            var u1 = xElement.Attribute("u1");
            var w0 = xElement.Attribute("w0");
            var w1 = xElement.Attribute("w1");
            return new ImplicitGradient(
                (x0 == null ? 0.00 : Double.Parse(x0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (x1 == null ? 1.00 : Double.Parse(x1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (y0 == null ? 0.00 : Double.Parse(y0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (y1 == null ? 1.00 : Double.Parse(y1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (z0 == null ? 0.00 : Double.Parse(z0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (z1 == null ? 1.00 : Double.Parse(z1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (v0 == null ? 0.00 : Double.Parse(v0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (v1 == null ? 1.00 : Double.Parse(v1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (u0 == null ? 0.00 : Double.Parse(u0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (u1 == null ? 1.00 : Double.Parse(u1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (w0 == null ? 0.00 : Double.Parse(w0.Value, NumberStyles.Any, CultureInfo.InvariantCulture)),
                (w1 == null ? 1.00 : Double.Parse(w1.Value, NumberStyles.Any, CultureInfo.InvariantCulture)));
        }

        public static ImplicitSphere SphereFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var xc = (xElement.Attribute("xc") != null ? xElement.Attribute("xc").Value : String.Empty);
            var yc = (xElement.Attribute("yc") != null ? xElement.Attribute("yc").Value : String.Empty);
            var zc = (xElement.Attribute("zc") != null ? xElement.Attribute("zc").Value : String.Empty);
            var vc = (xElement.Attribute("vc") != null ? xElement.Attribute("vc").Value : String.Empty);
            var uc = (xElement.Attribute("uc") != null ? xElement.Attribute("uc").Value : String.Empty);
            var wc = (xElement.Attribute("wc") != null ? xElement.Attribute("wc").Value : String.Empty);
            var radius = (xElement.Attribute("radius") != null ? xElement.Attribute("radius").Value : String.Empty);

            var sphere = new ImplicitSphere();
            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(xc))
            {
                if (chain.Modules.TryGetValue(xc, out source))
                    sphere.XCenter = source;
                else if (Double.TryParse(xc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.XCenter = value;
                else
                    throw new InvalidOperationException("Invalid xc value");
            }

            if (!String.IsNullOrEmpty(yc))
            {
                if (chain.Modules.TryGetValue(yc, out source))
                    sphere.YCenter = source;
                else if (Double.TryParse(yc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.YCenter = value;
                else
                    throw new InvalidOperationException("Invalid yc value");
            }

            if (!String.IsNullOrEmpty(zc))
            {

                if (chain.Modules.TryGetValue(zc, out source))
                    sphere.ZCenter = source;
                else if (Double.TryParse(zc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.ZCenter = value;
                else
                    throw new InvalidOperationException("Invalid zc value");
            }

            if (!String.IsNullOrEmpty(vc))
            {
                if (chain.Modules.TryGetValue(vc, out source))
                    sphere.VCenter = source;
                else if (Double.TryParse(vc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.VCenter = value;
                else
                    throw new InvalidOperationException("Invalid vc value");
            }

            if (!String.IsNullOrEmpty(uc))
            {
                if (chain.Modules.TryGetValue(uc, out source))
                    sphere.UCenter = source;
                else if (Double.TryParse(uc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.UCenter = value;
                else
                    throw new InvalidOperationException("Invalid uc value");
            }

            if (!String.IsNullOrEmpty(wc))
            {

                if (chain.Modules.TryGetValue(wc, out source))
                    sphere.WCenter = source;
                else if (Double.TryParse(wc, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.WCenter = value;
                else
                    throw new InvalidOperationException("Invalid wc value");
            }

            if (!String.IsNullOrEmpty(radius))
            {
                if (chain.Modules.TryGetValue(radius, out source))
                    sphere.Radius = source;
                else if (Double.TryParse(radius, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    sphere.Radius = value;
                else
                    throw new InvalidOperationException("Invalid radius value");
            }

            return sphere;
        }

        public static ImplicitScaleOffset ScaleOffsetFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var scaleString = (xElement.Attribute("scale") != null ? xElement.Attribute("scale").Value : String.Empty);
            var offsetString = (xElement.Attribute("offset") != null ? xElement.Attribute("offset").Value : String.Empty);

            ImplicitScaleOffset scaleOffset;
            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    scaleOffset = new ImplicitScaleOffset(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset = new ImplicitScaleOffset(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(scaleString))
            {
                if (chain.Modules.TryGetValue(scaleString, out source))
                    scaleOffset.Scale = source;
                else if (Double.TryParse(scaleString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset.Scale = value;
                else
                    throw new InvalidOperationException("Invalid scale value");
            }

            if (!String.IsNullOrEmpty(offsetString))
            {
                if (chain.Modules.TryGetValue(offsetString, out source))
                    scaleOffset.Offset = source;
                else if (Double.TryParse(offsetString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleOffset.Offset = value;
                else
                    throw new InvalidOperationException("Invalid offset value");
            }

            return scaleOffset;

        }

        public static ImplicitScaleDomain ScaleDomainFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var xs = (xElement.Attribute("xs") != null ? xElement.Attribute("xs").Value : String.Empty);
            var ys = (xElement.Attribute("ys") != null ? xElement.Attribute("ys").Value : String.Empty);
            var zs = (xElement.Attribute("zs") != null ? xElement.Attribute("zs").Value : String.Empty);
            var vs = (xElement.Attribute("vs") != null ? xElement.Attribute("vs").Value : String.Empty);
            var us = (xElement.Attribute("us") != null ? xElement.Attribute("us").Value : String.Empty);
            var ws = (xElement.Attribute("ws") != null ? xElement.Attribute("ws").Value : String.Empty);

            ImplicitScaleDomain scaleDomain;
            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    scaleDomain = new ImplicitScaleDomain(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain = new ImplicitScaleDomain(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(xs))
            {
                if (chain.Modules.TryGetValue(xs, out source))
                    scaleDomain.XScale = source;
                else if (Double.TryParse(xs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.XScale = value;
                else
                    throw new InvalidOperationException("Invalid xs value");
            }

            if (!String.IsNullOrEmpty(ys))
            {
                if (chain.Modules.TryGetValue(ys, out source))
                    scaleDomain.YScale = source;
                else if (Double.TryParse(ys, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.YScale = value;
                else
                    throw new InvalidOperationException("Invalid ys value");
            }

            if (!String.IsNullOrEmpty(zs))
            {

                if (chain.Modules.TryGetValue(zs, out source))
                    scaleDomain.ZScale = source;
                else if (Double.TryParse(zs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.ZScale = value;
                else
                    throw new InvalidOperationException("Invalid zs value");
            }

            if (!String.IsNullOrEmpty(vs))
            {
                if (chain.Modules.TryGetValue(vs, out source))
                    scaleDomain.VScale = source;
                else if (Double.TryParse(vs, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.VScale = value;
                else
                    throw new InvalidOperationException("Invalid vs value");
            }

            if (!String.IsNullOrEmpty(us))
            {
                if (chain.Modules.TryGetValue(us, out source))
                    scaleDomain.UScale = source;
                else if (Double.TryParse(us, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.UScale = value;
                else
                    throw new InvalidOperationException("Invalid us value");
            }

            if (!String.IsNullOrEmpty(ws))
            {

                if (chain.Modules.TryGetValue(ws, out source))
                    scaleDomain.WScale = source;
                else if (Double.TryParse(ws, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    scaleDomain.WScale = value;
                else
                    throw new InvalidOperationException("Invalid ws value");
            }

            return scaleDomain;
        }

        public static ImplicitTranslateDomain TranslateDomainFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var xa = (xElement.Attribute("xa") != null ? xElement.Attribute("xa").Value : String.Empty);
            var ya = (xElement.Attribute("ya") != null ? xElement.Attribute("ya").Value : String.Empty);
            var za = (xElement.Attribute("za") != null ? xElement.Attribute("za").Value : String.Empty);
            var va = (xElement.Attribute("va") != null ? xElement.Attribute("va").Value : String.Empty);
            var ua = (xElement.Attribute("ua") != null ? xElement.Attribute("ua").Value : String.Empty);
            var wa = (xElement.Attribute("wa") != null ? xElement.Attribute("wa").Value : String.Empty);

            ImplicitTranslateDomain translateDomain;

            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    translateDomain = new ImplicitTranslateDomain(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain = new ImplicitTranslateDomain(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(xa))
            {
                if (chain.Modules.TryGetValue(xa, out source))
                    translateDomain.XAxis = source;
                else if (Double.TryParse(xa, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.XAxis = value;
                else
                    throw new InvalidOperationException("Invalid xa value");
            }

            if (!String.IsNullOrEmpty(ya))
            {
                if (chain.Modules.TryGetValue(ya, out source))
                    translateDomain.YAxis = source;
                else if (Double.TryParse(ya, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.YAxis = value;
                else
                    throw new InvalidOperationException("Invalid ya value");
            }

            if (!String.IsNullOrEmpty(za))
            {
                if (chain.Modules.TryGetValue(za, out source))
                    translateDomain.ZAxis = source;
                else if (Double.TryParse(za, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.ZAxis = value;
                else
                    throw new InvalidOperationException("Invalid za value");
            }

            if (!String.IsNullOrEmpty(va))
            {
                if (chain.Modules.TryGetValue(va, out source))
                    translateDomain.VAxis = source;
                else if (Double.TryParse(va, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.VAxis = value;
                else
                    throw new InvalidOperationException("Invalid va value");
            }

            if (!String.IsNullOrEmpty(ua))
            {
                if (chain.Modules.TryGetValue(ua, out source))
                    translateDomain.UAxis = source;
                else if (Double.TryParse(ua, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.UAxis = value;
                else
                    throw new InvalidOperationException("Invalid ua value");
            }

            if (!String.IsNullOrEmpty(wa))
            {
                if (chain.Modules.TryGetValue(wa, out source))
                    translateDomain.WAxis = source;
                else if (Double.TryParse(wa, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    translateDomain.WAxis = value;
                else
                    throw new InvalidOperationException("Invalid wa value");
            }

            return translateDomain;
        }

        public static ImplicitSelect SelectFromXElement(ImplicitXmlChain chain, XElement xElement)
        {
            var sourceString = (xElement.Attribute("source") != null ? xElement.Attribute("source").Value : String.Empty);
            var low = (xElement.Attribute("low") != null ? xElement.Attribute("low").Value : String.Empty);
            var high = (xElement.Attribute("high") != null ? xElement.Attribute("high").Value : String.Empty);
            var falloff = (xElement.Attribute("falloff") != null ? xElement.Attribute("falloff").Value : String.Empty);
            var threshold = (xElement.Attribute("threshold") != null ? xElement.Attribute("threshold").Value : String.Empty);

            ImplicitSelect @select;

            ImplicitModuleBase source;
            Double value;

            if (!String.IsNullOrEmpty(sourceString))
            {
                if (chain.Modules.TryGetValue(sourceString, out source))
                    @select = new ImplicitSelect(source);
                else if (Double.TryParse(sourceString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select = new ImplicitSelect(value);
                else
                    throw new InvalidOperationException("Invalid source value");
            }
            else
                throw new InvalidOperationException("Missing source");

            if (!String.IsNullOrEmpty(low))
            {
                if (chain.Modules.TryGetValue(low, out source))
                    @select.Low = source;
                else if (Double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Low = value;
                else
                    throw new InvalidOperationException("Invalid low value");
            }

            if (!String.IsNullOrEmpty(high))
            {
                if (chain.Modules.TryGetValue(high, out source))
                    @select.High = source;
                else if (Double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.High = value;
                else
                    throw new InvalidOperationException("Invalid high value");
            }

            if (!String.IsNullOrEmpty(falloff))
            {
                if (chain.Modules.TryGetValue(falloff, out source))
                    @select.Falloff = source;
                else if (Double.TryParse(falloff, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Falloff = value;
                else
                    throw new InvalidOperationException("Invalid falloff value");
            }

            if (!String.IsNullOrEmpty(threshold))
            {
                if (chain.Modules.TryGetValue(threshold, out source))
                    @select.Threshold = source;
                else if (Double.TryParse(threshold, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    @select.Threshold = value;
                else
                    throw new InvalidOperationException("Invalid threshold value");
            }

            return @select;
        }

        private ImplicitXmlChain() { }

        public Dictionary<String, ImplicitModuleBase> Modules { get; private set; } 

        public ImplicitModuleBase Source { get; private set; }

        public override Double Get(Double x, Double y)
        {
            return this.Source.Get(x, y);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return this.Source.Get(x, y, z);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return this.Source.Get(x, y, z, w);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return this.Source.Get(x, y, z, w, u, v);
        }
    }
}
