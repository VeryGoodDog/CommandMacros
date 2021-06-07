using System;
using static Vintagestory.API.Client.GuiStyle;
using static Vintagestory.API.Client.ElementBounds;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Vintagestory.API.Client;

namespace CommandMacros {
	public class GuiDialogAliasEditor : GuiDialog {
		private AliasManager AliasMan;
		private List<ListCellEntry> cellList = new List<ListCellEntry>();
		private GuiElementTextArea textArea;
		private int selectedCellIndex = -1;

		public GuiDialogAliasEditor(ICoreClientAPI capi) : base(capi) {
			AliasMan = capi.ModLoader.GetModSystem<AliasMod>().AliasMan;
		}

		
		private void LoadAliases() {
			Console.WriteLine("LoadAliases");
			cellList.Clear();
			foreach (var alias in AliasMan) {
				cellList.Add(new ListCellEntry() {
					Data = alias,
					Title = alias.trigger,
					TitleFont = CairoFont.WhiteSmallishText()
				});
			}
		}

		public override string ToggleKeyCombinationCode => "opencmeditor";

		private void ComposeDialog() {
			Console.WriteLine("ComposeDialog");

			int height = 300; 
			int tableWidth = 150;
			int textAreaWidth = 200;

			var spacer = ElementStdBounds
				.TitleBar();

			var tableBounds = Fixed(0, 0, tableWidth, height);

			var textAreaBounds = Fixed(0, 0, textAreaWidth, height)
				.FixedRightOf(tableBounds, HalfPadding);

			var bodyBounds = Fill
				.WithSizing(ElementSizing.FitToChildren)
				.FixedUnder(spacer)
				.WithChildren(textAreaBounds, tableBounds);

			var bgBounds = Fill
				.WithFixedPadding(ElementToDialogPadding)
				.WithChildren(bodyBounds)
				.WithSizing(ElementSizing.FitToChildren);

			var dialogBounds = ElementStdBounds
				.AutosizedMainDialog
				.WithAlignment(EnumDialogArea.CenterMiddle);


			SingleComposer = capi.Gui.CreateCompo("aliaseditor", dialogBounds)
				.AddShadedDialogBG(bgBounds)
				.AddDialogTitleBar("Alias Editor", () => TryClose())
				.AddTextArea(textAreaBounds, OnTextChanged, CairoFont.TextInput(), "aliastextarea")
				.AddCellList(tableBounds,
					(cell, elBounds) => new GuiElementCell(capi, cell, elBounds) {ShowModifyIcons = false},
					OnMouseDownOnCell, OnMouseDownOnCell,
					cellList, "aliascellarea")
				.Compose();
			textArea = SingleComposer.GetTextArea("aliastextarea");
		}


		private void OnMouseDownOnCell(int cellIndex) {
			if (cellIndex == selectedCellIndex)
				return;
			var cell = cellList[cellIndex];
			var alias = cell.Data as Alias;
			var text = String.Join("\n", alias.commands);
			textArea.LoadValue(text);
			textArea.Enabled = true;
		}

		private void a(int c) {
			OnMouseDownOnCell(c);
		}

		public override void OnGuiOpened() {
			Console.WriteLine("OnGuiOpened");
			LoadAliases();
			try {
				ComposeDialog();
				textArea.Enabled = false;
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		public override void OnGuiClosed() {
			Console.WriteLine("OnGuiClosed");
			SingleComposer.Dispose();
		}

		private void OnTextChanged(string text) {
			// do something :)
		}
	}
}