tt.tip = tt.bh.ext({
	h:function()
	{
		var tipMsg = this.v.getTip(this.e,this.f,this.v,this.val,this.index);
		if (this.needHandle() && tipMsg) {
			this.render(tt.Conf.tipCls, tipMsg, "talentClose talentCloseTip", "talentSucInput");
		}
	}
});