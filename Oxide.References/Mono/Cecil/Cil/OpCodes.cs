using System;

namespace Mono.Cecil.Cil
{
	public static class OpCodes
	{
		internal readonly static OpCode[] OneByteOpCode;

		internal readonly static OpCode[] TwoBytesOpCode;

		public readonly static OpCode Nop;

		public readonly static OpCode Break;

		public readonly static OpCode Ldarg_0;

		public readonly static OpCode Ldarg_1;

		public readonly static OpCode Ldarg_2;

		public readonly static OpCode Ldarg_3;

		public readonly static OpCode Ldloc_0;

		public readonly static OpCode Ldloc_1;

		public readonly static OpCode Ldloc_2;

		public readonly static OpCode Ldloc_3;

		public readonly static OpCode Stloc_0;

		public readonly static OpCode Stloc_1;

		public readonly static OpCode Stloc_2;

		public readonly static OpCode Stloc_3;

		public readonly static OpCode Ldarg_S;

		public readonly static OpCode Ldarga_S;

		public readonly static OpCode Starg_S;

		public readonly static OpCode Ldloc_S;

		public readonly static OpCode Ldloca_S;

		public readonly static OpCode Stloc_S;

		public readonly static OpCode Ldnull;

		public readonly static OpCode Ldc_I4_M1;

		public readonly static OpCode Ldc_I4_0;

		public readonly static OpCode Ldc_I4_1;

		public readonly static OpCode Ldc_I4_2;

		public readonly static OpCode Ldc_I4_3;

		public readonly static OpCode Ldc_I4_4;

		public readonly static OpCode Ldc_I4_5;

		public readonly static OpCode Ldc_I4_6;

		public readonly static OpCode Ldc_I4_7;

		public readonly static OpCode Ldc_I4_8;

		public readonly static OpCode Ldc_I4_S;

		public readonly static OpCode Ldc_I4;

		public readonly static OpCode Ldc_I8;

		public readonly static OpCode Ldc_R4;

		public readonly static OpCode Ldc_R8;

		public readonly static OpCode Dup;

		public readonly static OpCode Pop;

		public readonly static OpCode Jmp;

		public readonly static OpCode Call;

		public readonly static OpCode Calli;

		public readonly static OpCode Ret;

		public readonly static OpCode Br_S;

		public readonly static OpCode Brfalse_S;

		public readonly static OpCode Brtrue_S;

		public readonly static OpCode Beq_S;

		public readonly static OpCode Bge_S;

		public readonly static OpCode Bgt_S;

		public readonly static OpCode Ble_S;

		public readonly static OpCode Blt_S;

		public readonly static OpCode Bne_Un_S;

		public readonly static OpCode Bge_Un_S;

		public readonly static OpCode Bgt_Un_S;

		public readonly static OpCode Ble_Un_S;

		public readonly static OpCode Blt_Un_S;

		public readonly static OpCode Br;

		public readonly static OpCode Brfalse;

		public readonly static OpCode Brtrue;

		public readonly static OpCode Beq;

		public readonly static OpCode Bge;

		public readonly static OpCode Bgt;

		public readonly static OpCode Ble;

		public readonly static OpCode Blt;

		public readonly static OpCode Bne_Un;

		public readonly static OpCode Bge_Un;

		public readonly static OpCode Bgt_Un;

		public readonly static OpCode Ble_Un;

		public readonly static OpCode Blt_Un;

		public readonly static OpCode Switch;

		public readonly static OpCode Ldind_I1;

		public readonly static OpCode Ldind_U1;

		public readonly static OpCode Ldind_I2;

		public readonly static OpCode Ldind_U2;

		public readonly static OpCode Ldind_I4;

		public readonly static OpCode Ldind_U4;

		public readonly static OpCode Ldind_I8;

		public readonly static OpCode Ldind_I;

		public readonly static OpCode Ldind_R4;

		public readonly static OpCode Ldind_R8;

		public readonly static OpCode Ldind_Ref;

		public readonly static OpCode Stind_Ref;

		public readonly static OpCode Stind_I1;

		public readonly static OpCode Stind_I2;

		public readonly static OpCode Stind_I4;

		public readonly static OpCode Stind_I8;

		public readonly static OpCode Stind_R4;

		public readonly static OpCode Stind_R8;

		public readonly static OpCode Add;

		public readonly static OpCode Sub;

		public readonly static OpCode Mul;

		public readonly static OpCode Div;

		public readonly static OpCode Div_Un;

		public readonly static OpCode Rem;

		public readonly static OpCode Rem_Un;

		public readonly static OpCode And;

		public readonly static OpCode Or;

		public readonly static OpCode Xor;

		public readonly static OpCode Shl;

		public readonly static OpCode Shr;

		public readonly static OpCode Shr_Un;

		public readonly static OpCode Neg;

		public readonly static OpCode Not;

		public readonly static OpCode Conv_I1;

		public readonly static OpCode Conv_I2;

		public readonly static OpCode Conv_I4;

		public readonly static OpCode Conv_I8;

		public readonly static OpCode Conv_R4;

		public readonly static OpCode Conv_R8;

		public readonly static OpCode Conv_U4;

		public readonly static OpCode Conv_U8;

		public readonly static OpCode Callvirt;

		public readonly static OpCode Cpobj;

		public readonly static OpCode Ldobj;

		public readonly static OpCode Ldstr;

		public readonly static OpCode Newobj;

		public readonly static OpCode Castclass;

		public readonly static OpCode Isinst;

		public readonly static OpCode Conv_R_Un;

		public readonly static OpCode Unbox;

		public readonly static OpCode Throw;

		public readonly static OpCode Ldfld;

		public readonly static OpCode Ldflda;

		public readonly static OpCode Stfld;

		public readonly static OpCode Ldsfld;

		public readonly static OpCode Ldsflda;

		public readonly static OpCode Stsfld;

		public readonly static OpCode Stobj;

		public readonly static OpCode Conv_Ovf_I1_Un;

		public readonly static OpCode Conv_Ovf_I2_Un;

		public readonly static OpCode Conv_Ovf_I4_Un;

		public readonly static OpCode Conv_Ovf_I8_Un;

		public readonly static OpCode Conv_Ovf_U1_Un;

		public readonly static OpCode Conv_Ovf_U2_Un;

		public readonly static OpCode Conv_Ovf_U4_Un;

		public readonly static OpCode Conv_Ovf_U8_Un;

		public readonly static OpCode Conv_Ovf_I_Un;

		public readonly static OpCode Conv_Ovf_U_Un;

		public readonly static OpCode Box;

		public readonly static OpCode Newarr;

		public readonly static OpCode Ldlen;

		public readonly static OpCode Ldelema;

		public readonly static OpCode Ldelem_I1;

		public readonly static OpCode Ldelem_U1;

		public readonly static OpCode Ldelem_I2;

		public readonly static OpCode Ldelem_U2;

		public readonly static OpCode Ldelem_I4;

		public readonly static OpCode Ldelem_U4;

		public readonly static OpCode Ldelem_I8;

		public readonly static OpCode Ldelem_I;

		public readonly static OpCode Ldelem_R4;

		public readonly static OpCode Ldelem_R8;

		public readonly static OpCode Ldelem_Ref;

		public readonly static OpCode Stelem_I;

		public readonly static OpCode Stelem_I1;

		public readonly static OpCode Stelem_I2;

		public readonly static OpCode Stelem_I4;

		public readonly static OpCode Stelem_I8;

		public readonly static OpCode Stelem_R4;

		public readonly static OpCode Stelem_R8;

		public readonly static OpCode Stelem_Ref;

		public readonly static OpCode Ldelem_Any;

		public readonly static OpCode Stelem_Any;

		public readonly static OpCode Unbox_Any;

		public readonly static OpCode Conv_Ovf_I1;

		public readonly static OpCode Conv_Ovf_U1;

		public readonly static OpCode Conv_Ovf_I2;

		public readonly static OpCode Conv_Ovf_U2;

		public readonly static OpCode Conv_Ovf_I4;

		public readonly static OpCode Conv_Ovf_U4;

		public readonly static OpCode Conv_Ovf_I8;

		public readonly static OpCode Conv_Ovf_U8;

		public readonly static OpCode Refanyval;

		public readonly static OpCode Ckfinite;

		public readonly static OpCode Mkrefany;

		public readonly static OpCode Ldtoken;

		public readonly static OpCode Conv_U2;

		public readonly static OpCode Conv_U1;

		public readonly static OpCode Conv_I;

		public readonly static OpCode Conv_Ovf_I;

		public readonly static OpCode Conv_Ovf_U;

		public readonly static OpCode Add_Ovf;

		public readonly static OpCode Add_Ovf_Un;

		public readonly static OpCode Mul_Ovf;

		public readonly static OpCode Mul_Ovf_Un;

		public readonly static OpCode Sub_Ovf;

		public readonly static OpCode Sub_Ovf_Un;

		public readonly static OpCode Endfinally;

		public readonly static OpCode Leave;

		public readonly static OpCode Leave_S;

		public readonly static OpCode Stind_I;

		public readonly static OpCode Conv_U;

		public readonly static OpCode Arglist;

		public readonly static OpCode Ceq;

		public readonly static OpCode Cgt;

		public readonly static OpCode Cgt_Un;

		public readonly static OpCode Clt;

		public readonly static OpCode Clt_Un;

		public readonly static OpCode Ldftn;

		public readonly static OpCode Ldvirtftn;

		public readonly static OpCode Ldarg;

		public readonly static OpCode Ldarga;

		public readonly static OpCode Starg;

		public readonly static OpCode Ldloc;

		public readonly static OpCode Ldloca;

		public readonly static OpCode Stloc;

		public readonly static OpCode Localloc;

		public readonly static OpCode Endfilter;

		public readonly static OpCode Unaligned;

		public readonly static OpCode Volatile;

		public readonly static OpCode Tail;

		public readonly static OpCode Initobj;

		public readonly static OpCode Constrained;

		public readonly static OpCode Cpblk;

		public readonly static OpCode Initblk;

		public readonly static OpCode No;

		public readonly static OpCode Rethrow;

		public readonly static OpCode Sizeof;

		public readonly static OpCode Refanytype;

		public readonly static OpCode Readonly;

		static OpCodes()
		{
			OpCodes.OneByteOpCode = new OpCode[225];
			OpCodes.TwoBytesOpCode = new OpCode[31];
			OpCodes.Nop = new OpCode(83886335, 318768389);
			OpCodes.Break = new OpCode(16843263, 318768389);
			OpCodes.Ldarg_0 = new OpCode(84017919, 335545601);
			OpCodes.Ldarg_1 = new OpCode(84083711, 335545601);
			OpCodes.Ldarg_2 = new OpCode(84149503, 335545601);
			OpCodes.Ldarg_3 = new OpCode(84215295, 335545601);
			OpCodes.Ldloc_0 = new OpCode(84281087, 335545601);
			OpCodes.Ldloc_1 = new OpCode(84346879, 335545601);
			OpCodes.Ldloc_2 = new OpCode(84412671, 335545601);
			OpCodes.Ldloc_3 = new OpCode(84478463, 335545601);
			OpCodes.Stloc_0 = new OpCode(84544255, 318833921);
			OpCodes.Stloc_1 = new OpCode(84610047, 318833921);
			OpCodes.Stloc_2 = new OpCode(84675839, 318833921);
			OpCodes.Stloc_3 = new OpCode(84741631, 318833921);
			OpCodes.Ldarg_S = new OpCode(84807423, 335549185);
			OpCodes.Ldarga_S = new OpCode(84873215, 369103617);
			OpCodes.Starg_S = new OpCode(84939007, 318837505);
			OpCodes.Ldloc_S = new OpCode(85004799, 335548929);
			OpCodes.Ldloca_S = new OpCode(85070591, 369103361);
			OpCodes.Stloc_S = new OpCode(85136383, 318837249);
			OpCodes.Ldnull = new OpCode(85202175, 436208901);
			OpCodes.Ldc_I4_M1 = new OpCode(85267967, 369100033);
			OpCodes.Ldc_I4_0 = new OpCode(85333759, 369100033);
			OpCodes.Ldc_I4_1 = new OpCode(85399551, 369100033);
			OpCodes.Ldc_I4_2 = new OpCode(85465343, 369100033);
			OpCodes.Ldc_I4_3 = new OpCode(85531135, 369100033);
			OpCodes.Ldc_I4_4 = new OpCode(85596927, 369100033);
			OpCodes.Ldc_I4_5 = new OpCode(85662719, 369100033);
			OpCodes.Ldc_I4_6 = new OpCode(85728511, 369100033);
			OpCodes.Ldc_I4_7 = new OpCode(85794303, 369100033);
			OpCodes.Ldc_I4_8 = new OpCode(85860095, 369100033);
			OpCodes.Ldc_I4_S = new OpCode(85925887, 369102849);
			OpCodes.Ldc_I4 = new OpCode(85991679, 369099269);
			OpCodes.Ldc_I8 = new OpCode(86057471, 385876741);
			OpCodes.Ldc_R4 = new OpCode(86123263, 402657541);
			OpCodes.Ldc_R8 = new OpCode(86189055, 419432197);
			OpCodes.Dup = new OpCode(86255103, 352388357);
			OpCodes.Pop = new OpCode(86320895, 318833925);
			OpCodes.Jmp = new OpCode(36055039, 318768133);
			OpCodes.Call = new OpCode(36120831, 471532549);
			OpCodes.Calli = new OpCode(36186623, 471533573);
			OpCodes.Ret = new OpCode(120138495, 320537861);
			OpCodes.Br_S = new OpCode(2763775, 318770945);
			OpCodes.Brfalse_S = new OpCode(53161215, 318967553);
			OpCodes.Brtrue_S = new OpCode(53227007, 318967553);
			OpCodes.Beq_S = new OpCode(53292799, 318902017);
			OpCodes.Bge_S = new OpCode(53358591, 318902017);
			OpCodes.Bgt_S = new OpCode(53424383, 318902017);
			OpCodes.Ble_S = new OpCode(53490175, 318902017);
			OpCodes.Blt_S = new OpCode(53555967, 318902017);
			OpCodes.Bne_Un_S = new OpCode(53621759, 318902017);
			OpCodes.Bge_Un_S = new OpCode(53687551, 318902017);
			OpCodes.Bgt_Un_S = new OpCode(53753343, 318902017);
			OpCodes.Ble_Un_S = new OpCode(53819135, 318902017);
			OpCodes.Blt_Un_S = new OpCode(53884927, 318902017);
			OpCodes.Br = new OpCode(3619071, 318767109);
			OpCodes.Brfalse = new OpCode(54016511, 318963717);
			OpCodes.Brtrue = new OpCode(54082303, 318963717);
			OpCodes.Beq = new OpCode(54148095, 318898177);
			OpCodes.Bge = new OpCode(54213887, 318898177);
			OpCodes.Bgt = new OpCode(54279679, 318898177);
			OpCodes.Ble = new OpCode(54345471, 318898177);
			OpCodes.Blt = new OpCode(54411263, 318898177);
			OpCodes.Bne_Un = new OpCode(54477055, 318898177);
			OpCodes.Bge_Un = new OpCode(54542847, 318898177);
			OpCodes.Bgt_Un = new OpCode(54608639, 318898177);
			OpCodes.Ble_Un = new OpCode(54674431, 318898177);
			OpCodes.Blt_Un = new OpCode(54740223, 318898177);
			OpCodes.Switch = new OpCode(54806015, 318966277);
			OpCodes.Ldind_I1 = new OpCode(88426239, 369296645);
			OpCodes.Ldind_U1 = new OpCode(88492031, 369296645);
			OpCodes.Ldind_I2 = new OpCode(88557823, 369296645);
			OpCodes.Ldind_U2 = new OpCode(88623615, 369296645);
			OpCodes.Ldind_I4 = new OpCode(88689407, 369296645);
			OpCodes.Ldind_U4 = new OpCode(88755199, 369296645);
			OpCodes.Ldind_I8 = new OpCode(88820991, 386073861);
			OpCodes.Ldind_I = new OpCode(88886783, 369296645);
			OpCodes.Ldind_R4 = new OpCode(88952575, 402851077);
			OpCodes.Ldind_R8 = new OpCode(89018367, 419628293);
			OpCodes.Ldind_Ref = new OpCode(89084159, 436405509);
			OpCodes.Stind_Ref = new OpCode(89149951, 319096069);
			OpCodes.Stind_I1 = new OpCode(89215743, 319096069);
			OpCodes.Stind_I2 = new OpCode(89281535, 319096069);
			OpCodes.Stind_I4 = new OpCode(89347327, 319096069);
			OpCodes.Stind_I8 = new OpCode(89413119, 319161605);
			OpCodes.Stind_R4 = new OpCode(89478911, 319292677);
			OpCodes.Stind_R8 = new OpCode(89544703, 319358213);
			OpCodes.Add = new OpCode(89610495, 335676677);
			OpCodes.Sub = new OpCode(89676287, 335676677);
			OpCodes.Mul = new OpCode(89742079, 335676677);
			OpCodes.Div = new OpCode(89807871, 335676677);
			OpCodes.Div_Un = new OpCode(89873663, 335676677);
			OpCodes.Rem = new OpCode(89939455, 335676677);
			OpCodes.Rem_Un = new OpCode(90005247, 335676677);
			OpCodes.And = new OpCode(90071039, 335676677);
			OpCodes.Or = new OpCode(90136831, 335676677);
			OpCodes.Xor = new OpCode(90202623, 335676677);
			OpCodes.Shl = new OpCode(90268415, 335676677);
			OpCodes.Shr = new OpCode(90334207, 335676677);
			OpCodes.Shr_Un = new OpCode(90399999, 335676677);
			OpCodes.Neg = new OpCode(90465791, 335611141);
			OpCodes.Not = new OpCode(90531583, 335611141);
			OpCodes.Conv_I1 = new OpCode(90597375, 369165573);
			OpCodes.Conv_I2 = new OpCode(90663167, 369165573);
			OpCodes.Conv_I4 = new OpCode(90728959, 369165573);
			OpCodes.Conv_I8 = new OpCode(90794751, 385942789);
			OpCodes.Conv_R4 = new OpCode(90860543, 402720005);
			OpCodes.Conv_R8 = new OpCode(90926335, 419497221);
			OpCodes.Conv_U4 = new OpCode(90992127, 369165573);
			OpCodes.Conv_U8 = new OpCode(91057919, 385942789);
			OpCodes.Callvirt = new OpCode(40792063, 471532547);
			OpCodes.Cpobj = new OpCode(91189503, 319097859);
			OpCodes.Ldobj = new OpCode(91255295, 335744003);
			OpCodes.Ldstr = new OpCode(91321087, 436209923);
			OpCodes.Newobj = new OpCode(41055231, 437978115);
			OpCodes.Castclass = new OpCode(91452671, 436866051);
			OpCodes.Isinst = new OpCode(91518463, 369757187);
			OpCodes.Conv_R_Un = new OpCode(91584255, 419497221);
			OpCodes.Unbox = new OpCode(91650559, 369757189);
			OpCodes.Throw = new OpCode(142047999, 319423747);
			OpCodes.Ldfld = new OpCode(91782143, 336199939);
			OpCodes.Ldflda = new OpCode(91847935, 369754371);
			OpCodes.Stfld = new OpCode(91913727, 319488259);
			OpCodes.Ldsfld = new OpCode(91979519, 335544579);
			OpCodes.Ldsflda = new OpCode(92045311, 369099011);
			OpCodes.Stsfld = new OpCode(92111103, 318832899);
			OpCodes.Stobj = new OpCode(92176895, 319032323);
			OpCodes.Conv_Ovf_I1_Un = new OpCode(92242687, 369165573);
			OpCodes.Conv_Ovf_I2_Un = new OpCode(92308479, 369165573);
			OpCodes.Conv_Ovf_I4_Un = new OpCode(92374271, 369165573);
			OpCodes.Conv_Ovf_I8_Un = new OpCode(92440063, 385942789);
			OpCodes.Conv_Ovf_U1_Un = new OpCode(92505855, 369165573);
			OpCodes.Conv_Ovf_U2_Un = new OpCode(92571647, 369165573);
			OpCodes.Conv_Ovf_U4_Un = new OpCode(92637439, 369165573);
			OpCodes.Conv_Ovf_U8_Un = new OpCode(92703231, 385942789);
			OpCodes.Conv_Ovf_I_Un = new OpCode(92769023, 369165573);
			OpCodes.Conv_Ovf_U_Un = new OpCode(92834815, 369165573);
			OpCodes.Box = new OpCode(92900607, 436276229);
			OpCodes.Newarr = new OpCode(92966399, 436407299);
			OpCodes.Ldlen = new OpCode(93032191, 369755395);
			OpCodes.Ldelema = new OpCode(93097983, 369888259);
			OpCodes.Ldelem_I1 = new OpCode(93163775, 369886467);
			OpCodes.Ldelem_U1 = new OpCode(93229567, 369886467);
			OpCodes.Ldelem_I2 = new OpCode(93295359, 369886467);
			OpCodes.Ldelem_U2 = new OpCode(93361151, 369886467);
			OpCodes.Ldelem_I4 = new OpCode(93426943, 369886467);
			OpCodes.Ldelem_U4 = new OpCode(93492735, 369886467);
			OpCodes.Ldelem_I8 = new OpCode(93558527, 386663683);
			OpCodes.Ldelem_I = new OpCode(93624319, 369886467);
			OpCodes.Ldelem_R4 = new OpCode(93690111, 403440899);
			OpCodes.Ldelem_R8 = new OpCode(93755903, 420218115);
			OpCodes.Ldelem_Ref = new OpCode(93821695, 436995331);
			OpCodes.Stelem_I = new OpCode(93887487, 319620355);
			OpCodes.Stelem_I1 = new OpCode(93953279, 319620355);
			OpCodes.Stelem_I2 = new OpCode(94019071, 319620355);
			OpCodes.Stelem_I4 = new OpCode(94084863, 319620355);
			OpCodes.Stelem_I8 = new OpCode(94150655, 319685891);
			OpCodes.Stelem_R4 = new OpCode(94216447, 319751427);
			OpCodes.Stelem_R8 = new OpCode(94282239, 319816963);
			OpCodes.Stelem_Ref = new OpCode(94348031, 319882499);
			OpCodes.Ldelem_Any = new OpCode(94413823, 336333827);
			OpCodes.Stelem_Any = new OpCode(94479615, 319884291);
			OpCodes.Unbox_Any = new OpCode(94545407, 336202755);
			OpCodes.Conv_Ovf_I1 = new OpCode(94614527, 369165573);
			OpCodes.Conv_Ovf_U1 = new OpCode(94680319, 369165573);
			OpCodes.Conv_Ovf_I2 = new OpCode(94746111, 369165573);
			OpCodes.Conv_Ovf_U2 = new OpCode(94811903, 369165573);
			OpCodes.Conv_Ovf_I4 = new OpCode(94877695, 369165573);
			OpCodes.Conv_Ovf_U4 = new OpCode(94943487, 369165573);
			OpCodes.Conv_Ovf_I8 = new OpCode(95009279, 385942789);
			OpCodes.Conv_Ovf_U8 = new OpCode(95075071, 385942789);
			OpCodes.Refanyval = new OpCode(95142655, 369167365);
			OpCodes.Ckfinite = new OpCode(95208447, 419497221);
			OpCodes.Mkrefany = new OpCode(95274751, 335744005);
			OpCodes.Ldtoken = new OpCode(95342847, 369101573);
			OpCodes.Conv_U2 = new OpCode(95408639, 369165573);
			OpCodes.Conv_U1 = new OpCode(95474431, 369165573);
			OpCodes.Conv_I = new OpCode(95540223, 369165573);
			OpCodes.Conv_Ovf_I = new OpCode(95606015, 369165573);
			OpCodes.Conv_Ovf_U = new OpCode(95671807, 369165573);
			OpCodes.Add_Ovf = new OpCode(95737599, 335676677);
			OpCodes.Add_Ovf_Un = new OpCode(95803391, 335676677);
			OpCodes.Mul_Ovf = new OpCode(95869183, 335676677);
			OpCodes.Mul_Ovf_Un = new OpCode(95934975, 335676677);
			OpCodes.Sub_Ovf = new OpCode(96000767, 335676677);
			OpCodes.Sub_Ovf_Un = new OpCode(96066559, 335676677);
			OpCodes.Endfinally = new OpCode(129686783, 318768389);
			OpCodes.Leave = new OpCode(12312063, 319946757);
			OpCodes.Leave_S = new OpCode(12377855, 319950593);
			OpCodes.Stind_I = new OpCode(96329727, 319096069);
			OpCodes.Conv_U = new OpCode(96395519, 369165573);
			OpCodes.Arglist = new OpCode(96403710, 369100037);
			OpCodes.Ceq = new OpCode(96469502, 369231109);
			OpCodes.Cgt = new OpCode(96535294, 369231109);
			OpCodes.Cgt_Un = new OpCode(96601086, 369231109);
			OpCodes.Clt = new OpCode(96666878, 369231109);
			OpCodes.Clt_Un = new OpCode(96732670, 369231109);
			OpCodes.Ldftn = new OpCode(96798462, 369099781);
			OpCodes.Ldvirtftn = new OpCode(96864254, 369755141);
			OpCodes.Ldarg = new OpCode(96930302, 335547909);
			OpCodes.Ldarga = new OpCode(96996094, 369102341);
			OpCodes.Starg = new OpCode(97061886, 318836229);
			OpCodes.Ldloc = new OpCode(97127678, 335547653);
			OpCodes.Ldloca = new OpCode(97193470, 369102085);
			OpCodes.Stloc = new OpCode(97259262, 318835973);
			OpCodes.Localloc = new OpCode(97325054, 369296645);
			OpCodes.Endfilter = new OpCode(130945534, 318964997);
			OpCodes.Unaligned = new OpCode(80679678, 318771204);
			OpCodes.Volatile = new OpCode(80745470, 318768388);
			OpCodes.Tail = new OpCode(80811262, 318768388);
			OpCodes.Initobj = new OpCode(97654270, 318966787);
			OpCodes.Constrained = new OpCode(97720062, 318770180);
			OpCodes.Cpblk = new OpCode(97785854, 319227141);
			OpCodes.Initblk = new OpCode(97851646, 319227141);
			OpCodes.No = new OpCode(97917438, 318771204);
			OpCodes.Rethrow = new OpCode(148314878, 318768387);
			OpCodes.Sizeof = new OpCode(98049278, 369101829);
			OpCodes.Refanytype = new OpCode(98115070, 369165573);
			OpCodes.Readonly = new OpCode(98180862, 318768388);
		}
	}
}