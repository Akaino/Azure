import { Version } from '@microsoft/sp-core-library';
import { IPropertyPaneConfiguration, 
  PropertyPaneTextField, 
  PropertyPaneCheckbox,
  PropertyPaneLabel,
  PropertyPaneLink,
  PropertyPaneSlider,
  PropertyPaneToggle,
  PropertyPaneDropdown } from '@microsoft/sp-property-pane';
import { BaseClientSideWebPart } from '@microsoft/sp-webpart-base';
import { escape } from '@microsoft/sp-lodash-subset';

import styles from './PropertyPaneWebPart.module.scss';
import * as strings from 'PropertyPaneWebPartStrings';

export interface IPropertyPaneWebPartProps {
  description: string;
  myCountry: string;
  numVisitedCountries: number;
  checkVal: true;
  dropdownItem: string;
}

export default class PropertyPaneWebPart extends BaseClientSideWebPart <IPropertyPaneWebPartProps> {

  public render(): void {
    this.domElement.innerHTML = `
      <div class="${ styles.propertyPane }">
        <div class="${ styles.container }">
          <div class="${ styles.row }">
            <div class="${ styles.column }">
              <span class="${ styles.title }">Welcome to SharePoint!</span>
              <p class="${ styles.subTitle }">Customize SharePoint experiences using Web Parts.</p>
              <p class="${ styles.description }">${escape(this.properties.description)}</p>
              <p class="${ styles.description }">Where i live: ${escape(this.properties.myCountry)}</p>
              <p class="${ styles.description }">Where i was (counted): ${this.properties.numVisitedCountries}</p>
              <p class="${ styles.description }">Checkbox: ${this.properties.checkVal}</p>
              <p class="${ styles.description }">Dropdown: ${this.properties.dropdownItem}</p>
              <a href="https://aka.ms/spfx" class="${ styles.button }">
                <span class="${ styles.label }">Learn more</span>
              </a>
            </div>
          </div>
        </div>
      </div>`;
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected get disableReactivePropertyChanges(): boolean {
    return false; //
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    const group1 = {
      groupName: strings.BasicGroupName,
      groupFields: [
        PropertyPaneTextField('description', {
          label: strings.DescriptionFieldLabel
        }),
        PropertyPaneTextField('myCountry', {
          label: 'Where I live:'
        }),
        PropertyPaneSlider('numVisitedCountries', {
          label: 'Number of continents I\'ve visited',  min: 1, max: 7, showValue: true,
        }),
        PropertyPaneCheckbox('checkVal', {
          text: "check me",
          checked: true
        }),
        PropertyPaneDropdown('dropdownItem', { 
          label: 'Ein Dropdown', 
          disabled: false,
          selectedKey: 'Red',
          options: [ 
            { key: 'Red', text: 'Red' }, 
            { key: 'Green', text: 'Green' }, 
            { key: 'DarkBlue', text: 'Dark blue'} 
          ] 
        })
      ]
    };

    const group2 = {
      groupName: "GR2",
      isCollapsed: true, //
      groupFields: [
        PropertyPaneTextField('description', {
          label: strings.DescriptionFieldLabel
        })
      ]
    };

    const page1 = {
      header: { description: strings.PropertyPaneDescription },
      groups: [ group1 ]
    };

    const page2 = {
      header: { description: "page 2" },
      displayGroupsAsAccordion: true, 
      groups: [ group1, group2 ]
    };

    return {
      pages: [ page1, page2 ]
    };
  }
}
