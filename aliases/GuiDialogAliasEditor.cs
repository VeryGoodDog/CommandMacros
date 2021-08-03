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
		private GuiElementTextInput textInput;

		public GuiDialogAliasEditor(ICoreClientAPI capi) : base(capi) {
			AliasMan = capi.ModLoader.GetModSystem<AliasMod>().AliasMan;
		}


		private void LoadAliases() {
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
			int height = 200;
			int tableWidth = 150;
			int textAreaWidth = 200;
			int headerHeight = 20;
			int labelWidth = 60;

			var spacer = ElementStdBounds
				.TitleBar();

			var tableBounds = Fixed(0, 0, tableWidth, height);

			var labelBounds = Fixed(0, 0, labelWidth, headerHeight);

			var nameInputBounds = Fixed(labelWidth, 0, textAreaWidth - labelWidth, headerHeight);

			var headerBounds = Fixed(0, 0, textAreaWidth, headerHeight)
				.WithChildren(labelBounds, nameInputBounds);

			var textAreaBounds = Fixed(0, 0, textAreaWidth, height)
				.FixedUnder(headerBounds, HalfPadding);

			var textContainerBounds = Fill
				.WithSizing(ElementSizing.FitToChildren)
				.FixedRightOf(tableBounds, HalfPadding)
				.WithChildren(textAreaBounds, headerBounds);

			var bodyBounds = Fill
				.WithSizing(ElementSizing.FitToChildren)
				.FixedUnder(spacer)
				.WithChildren(textContainerBounds, tableBounds);

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
				.AddStaticText("Alias:", CairoFont.WhiteSmallishText(), labelBounds)
				.AddTextInput(nameInputBounds, OnAliasNameChanged, CairoFont.TextInput(), "aliastextinput")
				.AddTextArea(textAreaBounds, OnAliasTextChanged, CairoFont.TextInput(), "aliastextarea")
				.AddCellList(tableBounds,
					(cell, elBounds) => new GuiElementCell(capi, cell, elBounds) {ShowModifyIcons = false},
					OnMouseDownOnCell, OnMouseDownOnCell,
					cellList, "aliascellarea")
				.Compose();
			textArea = SingleComposer.GetTextArea("aliastextarea");
			textArea.Enabled = false;
			textInput = SingleComposer.GetTextInput("aliastextinput");
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

		public override void OnGuiOpened() {
			LoadAliases();
			try {
				ComposeDialog();
				textArea.Enabled = false;
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		public override void OnGuiClosed() {
			SingleComposer?.Dispose();
		}

		private void OnAliasTextChanged(string text) {
			// do something :)
		}
		
		private void OnAliasNameChanged(string newName) { }
	}
}